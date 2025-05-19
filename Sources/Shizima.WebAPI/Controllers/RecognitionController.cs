using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("recognition")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class RecognitionController : Controller
    {
        private readonly RecognitionService _recognitionService;

        public RecognitionController(RecognitionService recognitionService) 
        {
            _recognitionService = recognitionService;
        }

        /// <summary>
        /// Load the music data on the server, to start recognition.
        /// </summary>
        /// <param name="data">The music data as byte's array.</param>
        /// <param name="userToken">The user token that request the recognition</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A boolean value based on the lenght of the data.</returns>
        [HttpPost("userToken/{userToken}", Name = "Send music data for recognition")]
        [ProducesResponseType<bool>(StatusCodes.Status200OK)]
        public async Task<ActionResult<MusicDto>> RecoguenizeMusic([FromBody] IEnumerable<byte> data, [FromQuery] string userToken, CancellationToken cancellationToken = default)
        {
            var result =  await _recognitionService.RecognizeAudioAsync(data, userToken, cancellationToken);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result.ToDto());
        }
    }
}