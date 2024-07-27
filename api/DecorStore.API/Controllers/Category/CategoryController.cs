using DecorStore.API.Controllers.Requests.Category;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery query)
        {
            _logger.LogInformation($"GetAllCategories endpoint is called, query parameters : {query}");
            var result = await _mediator.Send(query);
            _logger.LogInformation($"result count is {result.Count}");
            return Ok(result);
        }

        [HttpPost("createSection")]
        public async Task<IActionResult> CreateSection([FromQuery] CreateSectionCommand command)
        {
            _logger.LogInformation($"CreateSection endpoint is called, query parameters : {command}");
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
