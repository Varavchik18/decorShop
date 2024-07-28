using DecorStore.API.Controllers.Requests.Category;
using DecorStore.API.Controllers.Requests.Category.Commands;
using DecorStore.API.Controllers.Requests.Category.Commands.Section;
using DecorStore.API.Controllers.Requests.Category.Queries;
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

        [HttpGet("getSection/{idSection}")]
        public async Task<IActionResult> GetSectionById([FromRoute] int idSection)
        {
            var result = await _mediator.Send(new GetSectionByIdQuery()
            {
                SectionId = idSection
            });

            return Ok(result);
        }

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

        [HttpGet("getCategory/{idCategory}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int idCategory)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery()
            {
                CategoryId = idCategory
            });

            return Ok(result);
        }

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

        [HttpGet("getSubCategory/{idSubCategory}")]
        public async Task<IActionResult> GetSubCategoryById([FromRoute] int idSubCategory)
        {
            var result = await _mediator.Send(new GetSubCategoryByIdQuery()
            {
                SubCategoryId = idSubCategory
            });

            return Ok(result);
        }

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
