using MediatR;

namespace DecorStore.API.Controllers.Requests.Category.Commands
{
    public class DeleteSubCategoryCommand : IRequest<Unit>
    {
        public int SubCategoryId { get; set; }
        public int SectionId { get; set; }
        public int CategoryId { get; set; }
    }

    public class DeleteSubCategoryCommandHandler : IRequestHandler<DeleteSubCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteSubCategoryCommandHandler> _logger;

        public DeleteSubCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSubCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteSubCategoryCommand request, CancellationToken cancellationToken)
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

            var subcategory = category.Subcategories.FirstOrDefault(sc => sc.Id == request.SubCategoryId);
            if (subcategory == null)
            {
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SubcategoryNotFound });
            }

            category.Subcategories.Remove(subcategory);

            _logger.LogInformation($"Updating aggregate for section {request.SectionId}");
            await _unitOfWork.Categories.UpdateAsync(aggregate);
            _logger.LogInformation($"Completing unit of work for section {request.SectionId}");
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Subcategory with ID {request.SubCategoryId} deleted successfully from category {request.CategoryId} in section {request.SectionId}");

            return Unit.Value;
        }
    }
}
