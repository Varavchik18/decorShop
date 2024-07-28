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

            if (errorCodes.Any())
            {
                throw new DomainValidationException(errorCodes);
            }

            var aggregate = await _unitOfWork.Categories.GetBySectionIdAsync(request.SectionId);
            if (aggregate == null)
            {
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SectionNotFound });
            }

            var category = new DecorStore.BL.Models.Category { Name = request.Name, SectionId = request.SectionId };
            aggregate.AddCategory(category);

            await _unitOfWork.Categories.UpdateAsync(aggregate);
            await _unitOfWork.CompleteAsync();

            return category.Id;
        }
    }
}
