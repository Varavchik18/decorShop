
using AutoMapper;
using MediatR;

public class GetAllCategoriesQuery: IRequest<List<SectionDto>>
{
}

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<SectionDto>>
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly IMapper _mapper;


    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<SectionDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Categories.GetAllCategoriesAsync();

        if (result is null || result.Count == 0)
            throw new ArgumentNullException("There is no records for specified entity");

        return _mapper.Map<List<SectionDto>>(result);
    }
}

