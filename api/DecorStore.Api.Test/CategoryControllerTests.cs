using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using DecorStore.API.Controllers.Category;
using Microsoft.AspNetCore.Mvc;
using DecorStore.API.Controllers.Requests.Category.Queries;

namespace DecorStore.API.Tests
{
    public class CategoryControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<ILogger<CategoryController>> _mockLogger;
        private CategoryController _controller;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _controller = new CategoryController(_mockMediator.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllCategories_ReturnsOkResult_WithCategories()
        {
            // Arrange
            var mockCategories = new List<SectionDto>
            {
                new SectionDto
                {
                    Id = 1,
                    Name = "Test Section",
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto
                        {
                            Id = 1,
                            Name = "Test Category",
                            Subcategories = new List<SubcategoryDto>
                            {
                                new SubcategoryDto
                                {
                                    Id = 1,
                                    Name = "Test Subcategory",
                                    IconUrl = "https://example.com/icon.png"
                                }
                            }
                        }
                    }
                }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), default))
                         .ReturnsAsync(mockCategories);

            // Act
            var result = await _controller.GetAllCategories(new GetAllCategoriesQuery());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<List<SectionDto>>(okResult.Value);
            Assert.AreEqual(mockCategories, okResult.Value);
        }

        [Test]
        public void GetAllCategories_ThrowsException_WhenNoRecords()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), default))
                         .ThrowsAsync(new KeyNotFoundException("There are no records available."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetAllCategories(new GetAllCategoriesQuery()));
            Assert.AreEqual("There are no records available.", ex.Message);
        }

        [Test]
        public async Task GetSectionById_ReturnsOkResult_WithSectionDetails()
        {
            // Arrange
            var sectionId = 1;
            var mockSection = new SectionDto
            {
                Id = sectionId,
                Name = "Test Section",
                Categories = new List<CategoryDto>
                {
                    new CategoryDto
                    {
                        Id = 1,
                        Name = "Test Category",
                        Subcategories = new List<SubcategoryDto>
                        {
                            new SubcategoryDto
                            {
                                Id = 1,
                                Name = "Test Subcategory",
                                IconUrl = "https://example.com/icon.png"
                            }
                        }
                    }
                }
            };

            _mockMediator.Setup(m => m.Send(It.Is<GetSectionByIdQuery>(q => q.SectionId == sectionId), default))
                         .ReturnsAsync(mockSection);

            // Act
            var result = await _controller.GetSectionById(sectionId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<SectionDto>(okResult.Value);
            Assert.AreEqual(mockSection, okResult.Value);
        }
    }
}
