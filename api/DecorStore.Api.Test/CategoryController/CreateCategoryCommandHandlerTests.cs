using DecorStore.API.Controllers.Requests.Category;
using DecorStore.BL.Models;
using Microsoft.Extensions.Logging;

namespace DecorStore.API.Tests.CategoryController.CategoryTests
{
    [TestFixture]
    public class CreateCategoryCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<CreateCategoryCommandHandler>> _loggerMock;
        private CreateCategoryCommandHandler _createCategoryCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateCategoryCommandHandler>>();
            _createCategoryCommandHandler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateCategoryCommandHandler_ShouldReturnCategoryId_WhenCategoryIsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "New Category", SectionId = 1 };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);

            _unitOfWorkMock.Setup(u => u.Categories.IsCategoryNameUniqueInSectionAsync(command.Name, command.SectionId)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            _unitOfWorkMock.Setup(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>())).Callback<CategoryAggregate>(agg =>
            {
                agg.Categories.First().Id = 1;
            });

            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _createCategoryCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void CreateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryNameIsEmpty()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "", SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.IsCategoryNameUniqueInSectionAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(new CategoryAggregate(new Section { Id = 1, Name = "Section1" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNameIsRequired));
        }

        [Test]
        public void CreateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryNameIsNotUnique()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "Existing Category", SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.IsCategoryNameUniqueInSectionAsync(command.Name, command.SectionId)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(new CategoryAggregate(new Section { Id = 1, Name = "Section1" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNameAlreadyExistInSection));
        }

        [Test]
        public void CreateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "New Category", SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }
    }
}
