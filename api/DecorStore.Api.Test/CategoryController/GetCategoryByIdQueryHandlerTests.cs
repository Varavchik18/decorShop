using AutoMapper;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.CategoryTests
{
    [TestFixture]
    public class GetCategoryByIdQueryHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private GetCategoryByIdQueryHandler _getCategoryByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _getCategoryByIdQueryHandler = new GetCategoryByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetCategoryById_ShouldReturnCategoryDto_WhenCategoryExists()
        {
            // Arrange
            var category = new Category
            {
                Id = 1,
                Name = "Category1",
                Subcategories = new List<Subcategory>
                {
                    new Subcategory { Id = 1, Name = "Subcategory1", IconUrl = "icon1.png" },
                    new Subcategory { Id = 2, Name = "Subcategory2", IconUrl = "icon2.png" }
                }
            };

            _unitOfWorkMock.Setup(u => u.Categories.GetCategoryByIdAsync(category.Id)).ReturnsAsync(category);

            var categoryDto = new CategoryDto
            {
                Id = 1,
                Name = "Category1",
                Subcategories = new List<SubcategoryDto>
                {
                    new SubcategoryDto { Id = 1, Name = "Subcategory1", IconUrl = "icon1.png" },
                    new SubcategoryDto { Id = 2, Name = "Subcategory2", IconUrl = "icon2.png" }
                }
            };

            _mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(categoryDto);

            var query = new GetCategoryByIdQuery { CategoryId = category.Id };

            // Act
            var result = await _getCategoryByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CategoryDto>(result);
            Assert.AreEqual(categoryDto.Id, result.Id);
            Assert.AreEqual(categoryDto.Name, result.Name);
            Assert.AreEqual(categoryDto.Subcategories.Count, result.Subcategories.Count);
        }

        [Test]
        public void GetCategoryById_ShouldThrowDomainValidationException_WhenCategoryDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Categories.GetCategoryByIdAsync(It.IsAny<int>())).ReturnsAsync((Category)null);

            var query = new GetCategoryByIdQuery { CategoryId = 1 };

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _getCategoryByIdQueryHandler.Handle(query, CancellationToken.None));
            Assert.IsInstanceOf<DomainValidationException>(exception);
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNotFound));
        }

    }
}
