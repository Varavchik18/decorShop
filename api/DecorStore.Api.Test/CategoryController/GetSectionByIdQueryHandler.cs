using AutoMapper;
using DecorStore.API.Controllers.Requests.Category.Queries;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.SectionTests
{
    [TestFixture]
    public class GetSectionByIdQueryHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private GetSectionByIdQueryHandler _getSectionByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _getSectionByIdQueryHandler = new GetSectionByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetSectionById_ShouldReturnSectionDto_WhenSectionExists()
        {
            // Arrange
            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>
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
                }
            };

            _unitOfWorkMock.Setup(u => u.Categories.GetSectionBySectionIdAsync(section.Id)).ReturnsAsync(section);

            var sectionDto = new SectionDto
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
            };

            _mapperMock.Setup(m => m.Map<SectionDto>(section)).Returns(sectionDto);

            var query = new GetSectionByIdQuery { SectionId = section.Id };

            // Act
            var result = await _getSectionByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<SectionDto>(result);
            Assert.AreEqual(sectionDto.Id, result.Id);
            Assert.AreEqual(sectionDto.Name, result.Name);
            Assert.AreEqual(sectionDto.Categories.Count, result.Categories.Count);

            var categoriesList = result.Categories.ToList();
            Assert.AreEqual("Category1", categoriesList[0].Name);
            Assert.AreEqual(2, categoriesList[0].Subcategories.Count);

            var subcategoriesList = categoriesList[0].Subcategories.ToList();
            Assert.AreEqual("Subcategory1", subcategoriesList[0].Name);
            Assert.AreEqual("Subcategory2", subcategoriesList[1].Name);
        }

        [Test]
        public void GetSectionById_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Categories.GetSectionBySectionIdAsync(It.IsAny<int>())).ReturnsAsync((Section)null);

            var query = new GetSectionByIdQuery { SectionId = 1 };

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _getSectionByIdQueryHandler.Handle(query, CancellationToken.None));
            Assert.IsInstanceOf<DomainValidationException>(exception);
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }
    }
}
