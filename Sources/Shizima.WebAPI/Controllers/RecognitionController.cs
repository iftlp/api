using Microsoft.AspNetCore.Mvc;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("recognition")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class RecognitionController : Controller
    {
        public RecognitionController() 
        {
        }

        /// <summary>
        /// Load the music data on the server, to start recognition.
        /// </summary>
        /// <param name="data">The music data as byte's array.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A boolean value based on the lenght of the data.</returns>
        [HttpPost("load", Name = "Send music data for recognition")]
        [ProducesResponseType<bool>(StatusCodes.Status200OK)]
        public IActionResult RecoguenizeMusic([FromBody] IEnumerable<byte> data, CancellationToken cancellationToken = default)
        {
            bool debug = true;
            if (data.Count() > 1)
            {
                debug = false;
            }
            return debug ? new NotFoundObjectResult(debug) : new OkObjectResult(debug);
        }
    }
}