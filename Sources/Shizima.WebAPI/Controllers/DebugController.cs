using Microsoft.AspNetCore.Mvc;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("debug")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class DebugController
    {
        public DebugController()
        {
        }

        [HttpGet(Name = "GetDebug")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public IActionResult GetDebug(CancellationToken cancellationToken = default)
        {
            var value = "Hello World !";
            return value == null ? new NotFoundResult() : new OkObjectResult(value);
        }
    }
}
