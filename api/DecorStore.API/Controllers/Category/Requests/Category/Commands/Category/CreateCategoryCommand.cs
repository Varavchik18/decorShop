using DecorStore.BL.Models;
using MediatR;

namespace DecorStore.API.Controllers.Requests.Category
{
    public class CreateCategoryCommand : IRequest<int>
    {
        public string Name { get; set; }
        public int SectionId { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var errorCodes = new List<DomainErrorCodes>();

            if (String.IsNullOrWhiteSpace(request.Name))
                errorCodes.Add(DomainErrorCodes.CategoryNameIsRequired);

            if (!await _unitOfWork.Categories.IsCategoryNameUniqueInSectionAsync(request.Name, request.SectionId))
                errorCodes.Add(DomainErrorCodes.CategoryNameAlreadyExistInSection);


            var aggregate = await _unitOfWork.Categories.GetAggregateBySectionIdAsync(request.SectionId);
            if (aggregate == null)
            {
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SectionNotFound });
            }
            if (errorCodes.Any())
            {
                throw new DomainValidationException(errorCodes);
            }

            var category = new DecorStore.BL.Models.Category { Name = request.Name, SectionId = request.SectionId };
            aggregate.AddCategory(category);



            _logger.LogInformation($"Updating aggregate for section {request.SectionId}");
            await _unitOfWork.Categories.UpdateAsync(aggregate);
            _logger.LogInformation($"Completing unit of work for section {request.SectionId}");
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Completed unit of work for section {request.SectionId}");

            _logger.LogInformation($"Category {request.Name} created successfully with ID {category.Id} in section {request.SectionId}");


            return category.Id;
        }
    }
}
