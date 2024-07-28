using AutoMapper;
using MediatR;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public int CategoryId { get; set; }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId);

        if (category == null)
        {
            throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.CategoryNotFound });
        }

        return _mapper.Map<CategoryDto>(category);
    }
}