using AutoMapper;
using MediatR;

namespace DecorStore.API.Controllers.Requests.Category.Queries
{
    public class GetSubCategoryByIdQuery : IRequest<SubcategoryDto>
    {
        public int SubCategoryId { get; set; }
    }

    public class GetSubCategoryByIdQueryHandler : IRequestHandler<GetSubCategoryByIdQuery, SubcategoryDto>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetSubCategoryByIdQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            this._unitOfWork = unitOfWork;
        }

        public async Task<SubcategoryDto> Handle(GetSubCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var subCategory = await _unitOfWork.Categories.GetSubCategoryByIdAsync(request.SubCategoryId);

            if (subCategory == null)
            {
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SubcategoryNotFound });
            }

            return _mapper.Map<SubcategoryDto>(subCategory);
        }
    }
}
