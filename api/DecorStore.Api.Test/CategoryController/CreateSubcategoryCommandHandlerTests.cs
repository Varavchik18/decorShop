using DecorStore.API.Controllers.Requests.Category;
using DecorStore.BL.Models;
using Microsoft.Extensions.Logging;

namespace DecorStore.API.Tests.CategoryController.SubcategoryTests

{
    [TestFixture]
    public class CreateSubCategoryCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<CreateSubCategoryCommandHandler>> _loggerMock;
        private CreateSubCategoryCommandHandler _createSubCategoryCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateSubCategoryCommandHandler>>();
            _createSubCategoryCommandHandler = new CreateSubCategoryCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateSubCategoryCommandHandler_ShouldReturnSubcategoryId_WhenSubcategoryIsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateSubCategoryCommand { Name = "New Subcategory", CategoryId = 1, SectionId = 1, IconUrl = "icon.png" };

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

            _unitOfWorkMock.Setup(u => u.Categories.IsSubCategoryNameUniqueInCategoryAsync(command.Name, command.CategoryId)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            _unitOfWorkMock.Setup(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>())).Callback<CategoryAggregate>(agg =>
            {
                agg.Subcategories.First().Id = 1;
            });

            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _createSubCategoryCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void CreateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSubcategoryNameIsEmpty()
        {
            // Arrange
            var command = new CreateSubCategoryCommand { Name = "", CategoryId = 1, SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.IsSubCategoryNameUniqueInCategoryAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(new CategoryAggregate(new Section { Id = 1, Name = "Section1" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SubcategoryNameIsRequired));
        }

        [Test]
        public void CreateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSubcategoryNameIsNotUnique()
        {
            // Arrange
            var command = new CreateSubCategoryCommand { Name = "Existing Subcategory", CategoryId = 1, SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.IsSubCategoryNameUniqueInCategoryAsync(command.Name, command.CategoryId)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(new CategoryAggregate(new Section { Id = 1, Name = "Section1" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SubcategoryNameAlreadyExistInCategory));
        }

        [Test]
        public void CreateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new CreateSubCategoryCommand { Name = "New Subcategory", CategoryId = 1, SectionId = 999 };

            // Setup mocks to ensure previous conditions do not interfere
            _unitOfWorkMock.Setup(u => u.Categories.IsSubCategoryNameUniqueInCategoryAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }

        [Test]
        public void CreateSubCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryDoesNotExistInSection()
        {
            // Arrange
            var command = new CreateSubCategoryCommand { Name = "New Subcategory", CategoryId = 1, SectionId = 1 };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);

            // Setup mocks to ensure previous conditions do not interfere
            _unitOfWorkMock.Setup(u => u.Categories.IsSubCategoryNameUniqueInCategoryAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createSubCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNotFound));
        }
    }
}
