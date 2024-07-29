using Microsoft.Extensions.Logging;
using DecorStore.API.Controllers.Requests.Category.Commands;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.CategoryTests
{
    [TestFixture]
    public class DeleteCategoryCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<DeleteCategoryCommandHandler>> _loggerMock;
        private DeleteCategoryCommandHandler _deleteCategoryCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteCategoryCommandHandler>>();
            _deleteCategoryCommandHandler = new DeleteCategoryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task DeleteCategoryCommandHandler_ShouldDeleteCategory_WhenCategoryExists()
        {
            // Arrange
            var command = new DeleteCategoryCommand { CategoryId = 1, SectionId = 1 };

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
            var result = await _deleteCategoryCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(Unit.Value, result);
            Assert.IsFalse(aggregate.Section.Categories.Contains(category));
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void DeleteCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new DeleteCategoryCommand { CategoryId = 1, SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _deleteCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }

        [Test]
        public void DeleteCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryDoesNotExistInSection()
        {
            // Arrange
            var command = new DeleteCategoryCommand { CategoryId = 1, SectionId = 1 };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _deleteCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNotFound));
        }
    }
}
