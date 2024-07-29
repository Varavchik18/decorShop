using Microsoft.Extensions.Logging;
using DecorStore.API.Controllers.Requests.Category.Commands;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.SubcategoryTests
{
    [TestFixture]
    public class DeleteSubCategoryCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<DeleteSubCategoryCommandHandler>> _loggerMock;
        private DeleteSubCategoryCommandHandler _deleteSubCategoryCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteSubCategoryCommandHandler>>();
            _deleteSubCategoryCommandHandler = new DeleteSubCategoryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task DeleteSubCategoryCommandHandler_ShouldDeleteSubCategory_WhenSubCategoryExists()
        {
            // Arrange
            var command = new DeleteSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1 };

            var subcategory = new Subcategory
            {
                Id = 1,
                Name = "Subcategory1",
                IconUrl = "icon1.png"
            };

            var category = new Category
            {
                Id = 1,
                Name = "Category1",
                Subcategories = new List<Subcategory> { subcategory }
            };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category> { category }
            };

            var aggregate = new CategoryAggregate(section);
            aggregate.AddCategory(category);
            aggregate.AddSubcategory(subcategory);

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);
            _unitOfWorkMock.Setup(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>())).Verifiable();
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _deleteSubCategoryCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(Unit.Value, result);
            Assert.IsFalse(category.Subcategories.Contains(subcategory));
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void DeleteSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new DeleteSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _deleteSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }

        [Test]
        public void DeleteSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var command = new DeleteSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1 };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _deleteSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNotFound));
        }

        [Test]
        public void DeleteSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSubCategoryDoesNotExist()
        {
            // Arrange
            var command = new DeleteSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1 };

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
            aggregate.AddCategory(category);

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _deleteSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SubcategoryNotFound));
        }
    }
}
