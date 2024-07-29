using MediatR;
using System.Linq;

namespace DecorStore.API.Controllers.Requests.Category.Commands
{
    public class UpdateCategoryCommand : IRequest<int>
    {
        public int CategoryId { get; set; }
        public int SectionId { get; set; }
        public string Name { get; set; }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, int>
    {
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(ILogger<UpdateCategoryCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var errorCodes = new List<DomainErrorCodes>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errorCodes.Add(DomainErrorCodes.CategoryNameIsRequired);

            var aggregate = await _unitOfWork.Categories.GetAggregateBySectionIdAsync(request.SectionId);

            if (aggregate is null)
            {
                errorCodes.Add(DomainErrorCodes.SectionNotFound);
            }
            else
            {
                var category = aggregate.Categories.SingleOrDefault(x => x.Id == request.CategoryId);

                if (category is null)
                {
                    errorCodes.Add(DomainErrorCodes.CategoryNotFound);
                }
                else
                {
                    category.Name = request.Name;
                }
            }

            if (errorCodes.Any())
            {
                throw new DomainValidationException(errorCodes);
            }

            _logger.LogInformation($"Updating aggregate for section {request.SectionId}");
            await _unitOfWork.Categories.UpdateAsync(aggregate);
            _logger.LogInformation($"Completing unit of work for section {request.SectionId}");
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Category with ID {request.CategoryId} updated successfully in section {request.SectionId}");

            return request.CategoryId;



        }
    }
}
