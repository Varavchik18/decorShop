using Microsoft.Extensions.Logging;
using DecorStore.API.Controllers.Requests.Category.Commands;
using DecorStore.BL.Models;

namespace DecorStore.API.Tests.CategoryController.SectionTests
{
    [TestFixture]
    public class UpdateSectionCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<UpdateSectionCommandHandler>> _loggerMock;
        private UpdateSectionCommandHandler _updateSectionCommandHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateSectionCommandHandler>>();
            _updateSectionCommandHandler = new UpdateSectionCommandHandler(_loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Test]
        public async Task UpdateSectionCommandHandler_ShouldReturnSectionId_WhenSectionIsUpdatedSuccessfully()
        {
            // Arrange
            var command = new UpdateSectionCommand { SectionId = 1, Name = "Updated Section" };

            var section = new Section
            {
                Id = 1,
                Name = "Section1",
                Categories = new List<Category>()
            };

            var aggregate = new CategoryAggregate(section);

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync(aggregate);
            _unitOfWorkMock.Setup(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>())).Verifiable();
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _updateSectionCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual("Updated Section", section.Name);
            _unitOfWorkMock.Verify(u => u.Categories.UpdateAsync(It.IsAny<CategoryAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public void UpdateSectionCommandHandler_ShouldThrowDomainValidationException_WhenSectionNameIsEmpty()
        {
            // Arrange
            var command = new UpdateSectionCommand { SectionId = 1, Name = "" };

            var aggregate = new CategoryAggregate(new Section { Id = 1, Name = "Section1" });
            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(It.IsAny<int>())).ReturnsAsync(aggregate);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateSectionCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.CategoryNameIsRequired));
        }

        [Test]
        public void UpdateSectionCommandHandler_ShouldThrowDomainValidationException_WhenSectionDoesNotExist()
        {
            // Arrange
            var command = new UpdateSectionCommand { SectionId = 1, Name = "Updated Section" };

            _unitOfWorkMock.Setup(u => u.Categories.GetAggregateBySectionIdAsync(command.SectionId)).ReturnsAsync((CategoryAggregate)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainValidationException>(async () => await _updateSectionCommandHandler.Handle(command, CancellationToken.None));
            Assert.That(exception.ErrorCodes, Contains.Item(DomainErrorCodes.SectionNotFound));
        }
    }
}
