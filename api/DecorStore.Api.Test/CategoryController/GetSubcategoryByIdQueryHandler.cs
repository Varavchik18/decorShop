using AutoMapper;
using DecorStore.API.Controllers.Requests.Category.Queries;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.SubcategoryTests
{
    [TestFixture]
    public class GetSubCategoryByIdQueryHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private GetSubCategoryByIdQueryHandler _getSubCategoryByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _getSubCategoryByIdQueryHandler = new GetSubCategoryByIdQueryHandler(_mapperMock.Object, _unitOfWorkMock.Object);
        }

        [Test]
        public async Task GetSubCategoryByIdQueryHandler_ShouldReturnSubcategoryDto_WhenSubcategoryExists()
        {
            // Arrange
            var subcategory = new Subcategory
            {
                Id = 1,
                Name = "Subcategory1",
                IconUrl = "icon1.png"
            };

            _unitOfWorkMock.Setup(u => u.Categories.GetSubCategoryByIdAsync(subcategory.Id)).ReturnsAsync(subcategory);

            var subcategoryDto = new SubcategoryDto
            {
                Id = 1,
                Name = "Subcategory1",
                IconUrl = "icon1.png"
            };

            _mapperMock.Setup(m => m.Map<SubcategoryDto>(subcategory)).Returns(subcategoryDto);

            var query = new GetSubCategoryByIdQuery { SubCategoryId = subcategory.Id };

            // Act
            var result = await _getSubCategoryByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<SubcategoryDto>(result);
            Assert.AreEqual(subcategoryDto.Id, result.Id);
            Assert.AreEqual(subcategoryDto.Name, result.Name);
            Assert.AreEqual(subcategoryDto.IconUrl, result.IconUrl);
        }

        [Test]
        public void GetSubCategoryByIdQueryHandler_ShouldThrowDomainValidationException_WhenSubcategoryDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Categories.GetSubCategoryByIdAsync(It.IsAny<int>())).ReturnsAsync((Subcategory)null);

            var query = new GetSubCategoryByIdQuery { SubCategoryId = 1 };

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _getSubCategoryByIdQueryHandler.Handle(query, CancellationToken.None));
            Assert.IsInstanceOf<DomainValidationException>(exception);
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SubcategoryNotFound));
        }
    }
}
