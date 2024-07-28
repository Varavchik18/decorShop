using MediatR;

namespace DecorStore.API.Controllers.Requests.Category.Commands.Section
{
    public class DeleteSectionCommand : IRequest<Unit>
    {
        public int SectionId { get; set; }
    }

    public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteSectionCommandHandler> _logger;

        public DeleteSectionCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSectionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting section with ID {request.SectionId}");

            var aggregate = await _unitOfWork.Categories.GetBySectionIdAsync(request.SectionId);
            if (aggregate == null)
            {
                _logger.LogWarning($"Section {request.SectionId} not found");
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SectionNotFound });
            }

            _unitOfWork.Categories.RemoveAggregate(aggregate);

            _logger.LogInformation($"Completing unit of work for section {request.SectionId}");
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Section with ID {request.SectionId} deleted successfully");

            return Unit.Value;
        }
    }
}
