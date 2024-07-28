using DecorStore.API.Controllers.Requests.Category;
using Microsoft.Extensions.Logging;

namespace DecorStore.API.Tests.CategoryController.SectionTests

{
    [TestFixture]
    public class CreateSectionCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<CreateSectionCommandHandler>> _loggerMock;
        private CreateSectionCommandHandler _createSectionCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateSectionCommandHandler>>();
            _createSectionCommandHandler = new CreateSectionCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateSectionCommandHandler_ShouldReturnSectionId_WhenSectionIsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateSectionCommand { Title = "New Section" };

            _unitOfWorkMock.Setup(u => u.Categories.IsSectionNameUniqueAsync(command.Title)).ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.Categories.AddAsync(It.IsAny<CategoryAggregate>())).Callback<CategoryAggregate>(agg =>
            {
                agg.Section.Id = 1;
            });

            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _createSectionCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            _unitOfWorkMock.Verify(u => u.Categories.AddAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void CreateSectionCommandHandler_ShouldThrowDomainValidationException_WhenSectionNameIsNotUnique()
        {
            // Arrange
            var command = new CreateSectionCommand { Title = "Existing Section" };

            _unitOfWorkMock.Setup(u => u.Categories.IsSectionNameUniqueAsync(command.Title)).ReturnsAsync(false);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createSectionCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNameAlreadyExist));
        }
    }
}
