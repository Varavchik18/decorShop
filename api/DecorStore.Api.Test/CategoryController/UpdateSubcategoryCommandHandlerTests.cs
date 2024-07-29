using Microsoft.Extensions.Logging;
using DecorStore.API.Controllers.Requests.Category.Commands;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.SubcategoryTests
{
    [TestFixture]
    public class UpdateSubCategoryCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<UpdateSubCategoryCommandHandler>> _loggerMock;
        private UpdateSubCategoryCommandHandler _updateSubCategoryCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateSubCategoryCommandHandler>>();
            _updateSubCategoryCommandHandler = new UpdateSubCategoryCommandHandler(_loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Test]
        public async Task UpdateSubCategoryCommandHandler_ShouldReturnSubCategoryId_WhenSubCategoryIsUpdatedSuccessfully()
        {
            // Arrange
            var command = new UpdateSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1, Name = "Updated Subcategory", IconUrl = "updated-icon.png" };

            var subCategory = new Subcategory
            {
                Id = 1,
                Name = "Subcategory1",
                IconUrl = "icon1.png"
            };

            var category = new Category
            {
                Id = 1,
                Name = "Category1",
                Subcategories = new List<Subcategory> { subCategory }
            };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category> { category }
            };

            var aggregate = new CategoryAggregate(section);
            aggregate.AddCategory(category);
            aggregate.AddSubcategory(subCategory);

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);
            _unitOfWorkMock.Setup(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>())).Verifiable();
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _updateSubCategoryCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual("Updated Subcategory", subCategory.Name);
            Assert.AreEqual("updated-icon.png", subCategory.IconUrl);
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void UpdateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSubCategoryNameIsEmpty()
        {
            // Arrange
            var command = new UpdateSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1, Name = "", IconUrl = "updated-icon.png" };

            var subCategory = new Subcategory
            {
                Id = 1,
                Name = "Subcategory1",
                IconUrl = "icon1.png"
            };

            var category = new Category
            {
                Id = 1,
                Name = "Category1",
                Subcategories = new List<Subcategory> { subCategory }
            };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category> { category }
            };

            var aggregate = new CategoryAggregate(section);

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNameIsRequired));
        }

        [Test]
        public void UpdateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new UpdateSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1, Name = "Updated Subcategory", IconUrl = "updated-icon.png" };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }

        [Test]
        public void UpdateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var command = new UpdateSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1, Name = "Updated Subcategory", IconUrl = "updated-icon.png" };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNotFound));
        }

        [Test]
        public void UpdateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSubCategoryDoesNotExist()
        {
            // Arrange
            var command = new UpdateSubCategoryCommand { SubCategoryId = 1, CategoryId = 1, SectionId = 1, Name = "Updated Subcategory", IconUrl = "updated-icon.png" };

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
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SubcategoryNotFound));
        }
    }
}
