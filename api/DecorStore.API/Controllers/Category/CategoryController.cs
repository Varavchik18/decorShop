using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DecorStore.API.Controllers.Category
{
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator; 
        private readonly ILogger<CategoryController> _logger;


        public CategoryController(IMediator mediator, ILogger<CategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery query)
        {
            _logger.LogInformation($"GetAllCategories endpoint is called, query parameters : {query}");
            try
            {
                var result = await _mediator.Send(query);
                _logger.LogInformation($"result count is {result.Count}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($" {ex.Data} , {ex.Message}, {ex.InnerException}, {ex.Source} ");
                return BadRequest(ex.Message + " " +  ex.Source);
            }
        }
    }
}
