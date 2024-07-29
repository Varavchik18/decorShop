using MediatR;
using System.Linq;

namespace DecorStore.API.Controllers.Requests.Category.Commands
{
    public class UpdateSubCategoryCommand : IRequest<int>
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public int SectionId { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
    }

    public class UpdateSubCategoryCommandHandler : IRequestHandler<UpdateSubCategoryCommand, int>
    {
        private readonly ILogger<UpdateSubCategoryCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSubCategoryCommandHandler(ILogger<UpdateSubCategoryCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(UpdateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var errorCodes = new List<DomainErrorCodes>();

            if (String.IsNullOrWhiteSpace(request.Name))
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
                    var subCategory = category.Subcategories.SingleOrDefault(x => x.Id == request.SubCategoryId);
                    if (subCategory is null)
                    {
                        errorCodes.Add(DomainErrorCodes.SubcategoryNotFound);
                    }
                    else
                    {
                        subCategory.Name = request.Name;
                        subCategory.IconUrl = request.IconUrl;
                    }
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

            _logger.LogInformation($"Subcategory with ID {request.SubCategoryId} updated successfully in category {request.CategoryId} in section {request.SectionId}");

            return request.SubCategoryId;
        }
    }
}
