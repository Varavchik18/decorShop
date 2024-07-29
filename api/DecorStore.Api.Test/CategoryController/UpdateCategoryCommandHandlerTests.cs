using Microsoft.Extensions.Logging;
using DecorStore.API.Controllers.Requests.Category.Commands;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.CategoryTests
{
    [TestFixture]
    public class UpdateCategoryCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<UpdateCategoryCommandHandler>> _loggerMock;
        private UpdateCategoryCommandHandler _updateCategoryCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateCategoryCommandHandler>>();
            _updateCategoryCommandHandler = new UpdateCategoryCommandHandler(_loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Test]
        public async Task UpdateCategoryCommandHandler_ShouldReturnCategoryId_WhenCategoryIsUpdatedSuccessfully()
        {
            // Arrange
            var command = new UpdateCategoryCommand { CategoryId = 1, SectionId = 1, Name = "Updated Category" };

            var category = new Category
            {
                Id = 1,
                Name = "Category1",
                Subcategories = new List<Subcategory>()
            };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category> { category }
            };

            var aggregate = new CategoryAggregate(section);
            aggregate.AddCategory(category); // Ensure category is added to the aggregate

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);
            _unitOfWorkMock.Setup(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>())).Verifiable();
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _updateCategoryCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual("Updated Category", category.Name);
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void UpdateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryNameIsEmpty()
        {
            // Arrange
            var command = new UpdateCategoryCommand { CategoryId = 1, SectionId = 1, Name = "" };

            var aggregate = new CategoryAggregate(new Section { Id = 1, Name = "Section1" });
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNameIsRequired));
        }

        [Test]
        public void UpdateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new UpdateCategoryCommand { CategoryId = 1, SectionId = 1, Name = "Updated Category" };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }

        [Test]
        public void UpdateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryDoesNotExistInSection()
        {
            // Arrange
            var command = new UpdateCategoryCommand { CategoryId = 1, SectionId = 1, Name = "Updated Category" };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNotFound));
        }
    }
}
