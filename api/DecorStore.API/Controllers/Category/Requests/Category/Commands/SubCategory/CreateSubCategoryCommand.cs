using DecorStore.BL.Models;
using MediatR;

namespace DecorStore.API.Controllers.Requests.Category
{
    public class CreateSubCategoryCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string? IconUrl { get; set; }
        public int SectionId { get; set; }
        public int CategoryId { get; set; }
    }

    public class CreateSubCategoryCommandHandler : IRequestHandler<CreateSubCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSubCategoryCommandHandler> _logger;

        public CreateSubCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateSubCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var errorCodes = new List<DomainErrorCodes>();

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                errorCodes.Add(DomainErrorCodes.SubcategoryNameIsRequired);
            }

            if (!await _unitOfWork.Categories.IsSubCategoryNameUniqueInCategoryAsync(request.Name, request.CategoryId))
            {
                errorCodes.Add(DomainErrorCodes.SubcategoryNameAlreadyExistInCategory);
            }

            if (errorCodes.Any())
            {
                throw new DomainValidationException(errorCodes);
            }

            var aggregate = await _unitOfWork.Categories.GetAggregateBySectionIdAsync(request.SectionId);
            if (aggregate == null)
            {
                _logger.LogWarning($"Section {request.SectionId} not found");
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SectionNotFound });
            }

            var category = aggregate.Section.Categories.FirstOrDefault(c => c.Id == request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning($"Category {request.CategoryId} not found in section {request.SectionId}");
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.CategoryNotFound });
            }

            var subcategory = new Subcategory { Name = request.Name, CategoryId = request.CategoryId, IconUrl = request.IconUrl };
            aggregate.AddSubcategory(subcategory);

            _logger.LogInformation($"Updating aggregate for section {request.SectionId}");
            await _unitOfWork.Categories.UpdateAsync(aggregate);
            _logger.LogInformation($"Completing unit of work for section {request.SectionId}");
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Completed unit of work for section {request.SectionId}");

            _logger.LogInformation($"Subcategory {request.Name} created successfully with ID {subcategory.Id} in category {request.CategoryId} within section {request.SectionId}");

            return subcategory.Id;
        }
    }
}
