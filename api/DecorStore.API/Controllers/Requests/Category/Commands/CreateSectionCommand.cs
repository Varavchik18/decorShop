using DecorStore.Domain.Exceptions;
using MediatR;

namespace DecorStore.API.Controllers.Requests.Category
{
    public class CreateSectionCommand : IRequest<int>
    {
        public string Title { get; set; }
    }

    public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSectionCommandHandler> _logger;

        public CreateSectionCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateSectionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Creating section {request.Title}");
            var errorCodes = new List<DomainErrorCodes>();

            if (!await _unitOfWork.Categories.IsSectionNameUniqueAsync(request.Title))
                errorCodes.Add(DomainErrorCodes.SectionNameAlreadyExist);

            if (errorCodes.Any())
            {
                throw new DomainValidationException(errorCodes);
            }

            var section = new Section { Name = request.Title };
            await _unitOfWork.Categories.AddAsync(new CategoryAggregate(section));
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Section {request.Title} created successfully with ID {section.Id}");

            return section.Id;
        }
    }
}
