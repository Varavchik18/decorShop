using DecorStore.BL.Models;
using AutoMapper;
using System.Reflection.Metadata;
using DecorStore.API.Controllers.Requests.Category.Queries;
using DecorStore.API.Controllers.Requests.Category;
using Microsoft.Extensions.Logging;
using Moq;

namespace DecorStore.API.Tests
{
    [TestFixture]
    public class GetAllCategoriesQueryHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<CreateCategoryCommandHandler>> _createCategoryLoggerMock;
        private Mock<ILogger<CreateSectionCommandHandler>> _createSectionloggerMock;
        private GetAllCategoriesQueryHandler _getAllCategoriesQueryHandler;
        private GetCategoryByIdQueryHandler _getCategoryByIdQueryHandler;
        private GetSectionByIdQueryHandler _getSectionByIdQueryHandler;
        private GetSubCategoryByIdQueryHandler _getSubCategoryByIdQueryHandler;
        private CreateCategoryCommandHandler _createCategoryCommandHandler;
        private CreateSectionCommandHandler _createSectionCommandHandler;
        private Mock<ILogger<CreateSubCategoryCommandHandler>> _lcreateSubCategoryLoggerMock;
        private CreateSubCategoryCommandHandler _createSubCategoryCommandHandler;





        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _createCategoryLoggerMock = new Mock<ILogger<CreateCategoryCommandHandler>>();
            _createSectionloggerMock = new Mock<ILogger<CreateSectionCommandHandler>>();
            _lcreateSubCategoryLoggerMock = new Mock<ILogger<CreateSubCategoryCommandHandler>>();
            _createSubCategoryCommandHandler = new CreateSubCategoryCommandHandler(_unitOfWorkMock.Object, _lcreateSubCategoryLoggerMock.Object);
            _getAllCategoriesQueryHandler = new GetAllCategoriesQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            _getCategoryByIdQueryHandler = new GetCategoryByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            _getSectionByIdQueryHandler = new GetSectionByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            _getSubCategoryByIdQueryHandler = new GetSubCategoryByIdQueryHandler(_mapperMock.Object, _unitOfWorkMock.Object);
            _createCategoryCommandHandler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _createCategoryLoggerMock.Object);
            _createSectionCommandHandler = new CreateSectionCommandHandler(_unitOfWorkMock.Object, _createSectionloggerMock.Object);
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

        [Test]
        public async Task CreateCategoryCommandHandler_ShouldReturnCategoryId_WhenCategoryIsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "New Category", SectionId = 1 };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);

            _unitOfWorkMock.Setup(u => u.Categories.IsCategoryNameUniqueInSectionAsync(command.Name, command.SectionId)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);

            _unitOfWorkMock.Setup(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>())).Callback<CategoryAggregate>(agg =>
            {
                agg.Categories.First().Id = 1;
            });

            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _createCategoryCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void CreateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryNameIsEmpty()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "", SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.IsCategoryNameUniqueInSectionAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(new CategoryAggregate(new Section { Id = 1, Name = "Section1" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNameIsRequired));
        }

        [Test]
        public void CreateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenCategoryNameIsNotUnique()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "Existing Category", SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.IsCategoryNameUniqueInSectionAsync(command.Name, command.SectionId)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(new CategoryAggregate(new Section { Id = 1, Name = "Section1" }));

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNameAlreadyExistInSection));
        }

        [Test]
        public void CreateCategoryCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "New Category", SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createCategoryCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }

        [Test]
        public async Task CreateSectionCommandHandler_ShouldReturnSectionId_WhenSectionIsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateSectionCommand { Title = "New Section" };

            _unitOfWorkMock.Setup(u => u.Categories.IsSectionNameUniqueAsync(command.Title)).ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.Categories.AddAsync(It.IsAny<CategoryAggregate>())).Callback<CategoryAggregate>(agg =>
            {
                agg.Section.Id = 1;
            });

            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _createSectionCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            _unitOfWorkMock.Verify(u => u.Categories.AddAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void CreateSectionCommandHandler_ShouldThrowDomainValidationException_WhenSectionNameIsNotUnique()
        {
            // Arrange
            var command = new CreateSectionCommand { Title = "Existing Section" };

            _unitOfWorkMock.Setup(u => u.Categories.IsSectionNameUniqueAsync(command.Title)).ReturnsAsync(false);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _createSectionCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNameAlreadyExist));
        }
    }
}
