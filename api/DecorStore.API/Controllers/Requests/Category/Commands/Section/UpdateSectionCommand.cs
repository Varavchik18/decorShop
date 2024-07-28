using MediatR;
using System.Linq;

namespace DecorStore.API.Controllers.Requests.Category.Commands
{
    public class UpdateSectionCommand : IRequest<int>
    {
        public int SectionId { get; set; }
        public string Name { get; set; }
    }

    public class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, int>
    {
        private readonly ILogger<UpdateSectionCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSectionCommandHandler(ILogger<UpdateSectionCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            var errorCodes = new List<DomainErrorCodes>();

            if (String.IsNullOrWhiteSpace(request.Name))
                errorCodes.Add(DomainErrorCodes.CategoryNameIsRequired);

            var aggregate = await _unitOfWork.Categories.GetBySectionIdAsync(request.SectionId);

            if (aggregate is null)
            {
                errorCodes.Add(DomainErrorCodes.SectionNotFound);
            }

            if (errorCodes.Any())
            {
                throw new DomainValidationException(errorCodes);
            }

            aggregate.Section.Name = request.Name;

            _logger.LogInformation($"Updating aggregate for section {request.SectionId}");
            await _unitOfWork.Categories.UpdateAsync(aggregate);
            _logger.LogInformation($"Completing unit of work for section {request.SectionId}");
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Section with ID {request.SectionId} deleted successfully");

            return aggregate.Section.Id;
        }
    }
}
