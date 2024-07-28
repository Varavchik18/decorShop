using AutoMapper;
using MediatR;

namespace DecorStore.API.Controllers.Requests.Category.Queries
{
    public class GetSectionByIdQuery : IRequest<SectionDto>
    {
        public int SectionId { get; set; }
    }

    public class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, SectionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSectionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SectionDto> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
        {
            var section = await _unitOfWork.Categories.GetSectionBySectionIdAsync(request.SectionId);
            
            if(section == null)
            {
                throw new DomainValidationException(new List<DomainErrorCodes> { DomainErrorCodes.SectionNotFound });
            }

            return _mapper.Map<SectionDto>(section);
        }
    }
}
