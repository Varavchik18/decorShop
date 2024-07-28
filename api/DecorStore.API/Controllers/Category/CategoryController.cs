using DecorStore.API.Controllers.Requests.Category;
using DecorStore.API.Controllers.Requests.Category.Commands;
using DecorStore.API.Controllers.Requests.Category.Commands.Section;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DecorStore.API.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IMediator mediator, ILogger<CategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        #region Section endpoints
        
        [HttpPost("createSection")]
        public async Task<IActionResult> CreateSection([FromQuery] CreateSectionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("deleteSection")]
        public async Task<IActionResult> DeleteSection([FromQuery] DeleteSectionCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        #endregion

        #region Category endpoints
        
        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory([FromQuery] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("deleteCategory")]
        public async Task<IActionResult> DeleteCategory([FromQuery] DeleteCategoryCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
        #endregion

        #region SubCategory endpoints

        [HttpPost("createSubCategory")]
        public async Task<IActionResult> CreateSubCategory([FromQuery] CreateSubCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("deleteSubCategory")]
        public async Task<IActionResult> DeleteSubCategory([FromQuery] DeleteSubCategoryCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        #endregion
        
    }
}
