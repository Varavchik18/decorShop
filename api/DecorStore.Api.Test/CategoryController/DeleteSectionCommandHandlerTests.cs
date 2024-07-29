using Microsoft.Extensions.Logging;
using DecorStore.API.Controllers.Requests.Category.Commands.Section;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.SectionTests
{
    [TestFixture]
    public class DeleteSectionCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<DeleteSectionCommandHandler>> _loggerMock;
        private DeleteSectionCommandHandler _deleteSectionCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteSectionCommandHandler>>();
            _deleteSectionCommandHandler = new DeleteSectionCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task DeleteSectionCommandHandler_ShouldDeleteSectionAndItsCategoriesAndSubcategories_WhenSectionExists()
        {
            // Arrange
            var command = new DeleteSectionCommand { SectionId = 1 };

            var subcategories = new List<Subcategory>
            {
                new Subcategory { Id = 1, Name = "Subcategory1", IconUrl = "icon1.png" },
                new Subcategory { Id = 2, Name = "Subcategory2", IconUrl = "icon2.png" }
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1", Subcategories = subcategories }
            };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = categories
            };

            var aggregate = new CategoryAggregate(section);
            aggregate.AddCategory(categories.First());
            aggregate.AddSubcategory(subcategories.First());
            aggregate.AddSubcategory(subcategories.Last());

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);
            _unitOfWorkMock.Setup(u => u.Categories.RemoveAggregate(It.IsAny<CategoryAggregate>())).Verifiable();
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _deleteSectionCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(Unit.Value, result);
            _unitOfWorkMock.Verify(u => u.Categories.RemoveAggregate(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void DeleteSectionCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new DeleteSectionCommand { SectionId = 1 };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _deleteSectionCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }
    }
}
