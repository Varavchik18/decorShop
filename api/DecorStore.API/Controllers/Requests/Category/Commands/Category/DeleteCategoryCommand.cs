using MediatR;

namespace DecorStore.API.Controllers.Requests.Category.Commands
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public int CategoryId { get; set; }
        public int SectionId { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {

            var aggregate = await _unitOfWork.Categories.GetBySectionIdAsync(request.SectionId);
            if (aggregate == null)
            {
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SectionNotFound });
            }

            var category = aggregate.Categories.FirstOrDefault(c => c.Id == request.CategoryId);
            if (category == null)
            {
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.CategoryNotFound });
            }

            aggregate.Section.Categories.Remove(category);

            _logger.LogInformation($"Updating aggregate for section {request.SectionId}");
            await _unitOfWork.Categories.UpdateAsync(aggregate);
            _logger.LogInformation($"Completing unit of work for section {request.SectionId}");
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Category with ID {request.CategoryId} deleted successfully from section {request.SectionId}");

            return Unit.Value;
        }
    }
}
