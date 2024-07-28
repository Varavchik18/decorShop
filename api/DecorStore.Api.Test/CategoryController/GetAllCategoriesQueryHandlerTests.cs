using AutoMapper;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController
{
    [TestFixture]
    public class GetAllCategoriesQueryHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private GetAllCategoriesQueryHandler _getAllCategoriesQueryHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _getAllCategoriesQueryHandler = new GetAllCategoriesQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllCategories_ShouldReturnListOfSectionDto_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "Category1",
                    Subcategories = new List<Subcategory>
                    {
                        new Subcategory { Id = 1, Name = "Subcategory1", IconUrl = "icon1.png" },
                        new Subcategory { Id = 2, Name = "Subcategory2", IconUrl = "icon2.png" }
                    }
                }
            };

            var sections = new List<Section>
            {
                new Section
                {
                    Id = 1,
                    Name = "Section1",
                    Categories = categories
                }
            };

            _unitOfWorkMock.Setup(u => u.Categories.GetAllCategoriesAsync()).ReturnsAsync(sections);

            var sectionDtos = new List<SectionDto>
            {
                new SectionDto
                {
                    Id = 1,
                    Name = "Section1",
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto
                        {
                            Id = 1,
                            Name = "Category1",
                            Subcategories = new List<SubcategoryDto>
                            {
                                new SubcategoryDto { Id = 1, Name = "Subcategory1", IconUrl = "icon1.png" },
                                new SubcategoryDto { Id = 2, Name = "Subcategory2", IconUrl = "icon2.png" }
                            }
                        }
                    }
                }
            };

            _mapperMock.Setup(m => m.Map<List<SectionDto>>(sections)).Returns(sectionDtos);

            var query = new GetAllCategoriesQuery();

            // Act
            var result = await _getAllCategoriesQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<SectionDto>>(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Section1", result[0].Name);
            Assert.AreEqual(1, result[0].Categories.Count);

            var categoriesList = result[0].Categories.ToList();
            Assert.AreEqual("Category1", categoriesList[0].Name);
            Assert.AreEqual(2, categoriesList[0].Subcategories.Count);

            var subcategoriesList = categoriesList[0].Subcategories.ToList();
            Assert.AreEqual("Subcategory1", subcategoriesList[0].Name);
            Assert.AreEqual("Subcategory2", subcategoriesList[1].Name);
        }

        [Test]
        public void GetAllCategories_ShouldThrowKeyNotFoundException_WhenNoCategoriesExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Categories.GetAllCategoriesAsync()).ReturnsAsync(new List<Section>());

            var query = new GetAllCategoriesQuery();

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _getAllCategoriesQueryHandler.Handle(query, CancellationToken.None));
        }
    }
}
