using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;
using Shizima.WebAPI.Extentions.Mappers;

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
        /// Send music raw data for recognition, as a user.
        /// </summary>
        /// <param name="data">The music data as byte's array.</param>
        /// <param name="userToken">The user token that request the recognition</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The recognize music information.</returns>
        [HttpPost("rawData/{userToken}", Name = "Send raw music data for recognition, as a user")]
        [ProducesResponseType<HistoryDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<HistoryDto>> RecognizeAudioRawDataUserAsync([FromBody] IEnumerable<byte> data, [FromRoute] string userToken, CancellationToken cancellationToken = default)
        {
            var result =  await _recognitionService.RecognizeAudioAsUserAsync(data, userToken, cancellationToken);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result.ToDto());
        }
        
        /// <summary>
        /// Send music file data for recognition, as a user.
        /// </summary>
        /// <param name="audioFile">The music .wav file.</param>
        /// <param name="userToken">The user token that request the recognition</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The recognize music information.</returns>
        [HttpPost("wavFile/{userToken}", Name = "Send music file data for recognition, as a user")]
        [ProducesResponseType<HistoryDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<HistoryDto>> RecognizeAudioFileDataUserAsync([FromForm]IFormFile audioFile, [FromRoute] string userToken, CancellationToken cancellationToken = default)
        {
            var dataBytes = await audioFile.ToBytes();
            var result =  await _recognitionService.RecognizeAudioAsUserAsync(dataBytes, userToken, cancellationToken);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result.ToDto());
        }

        /// <summary>
        /// Send music raw data for recognition.
        /// </summary>
        /// <param name="data">The music data as byte's array.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The recognize music information.</returns>
        [HttpPost("rawData", Name = "Send music raw data for recognition")]
        [ProducesResponseType<MusicDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<MusicDto>> RecognizeAudioRawDataAsync([FromBody] IEnumerable<byte> data, CancellationToken cancellationToken = default)
        {
            var result =  await _recognitionService.RecognizeAudioAsync(data, cancellationToken);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result.ToDto());
        }
        
        /// <summary>
        /// Send music wav file for recognition.
        /// </summary>
        /// <param name="audioFile">The music .wav file.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The recognize music information.</returns>
        [HttpPost("wavFile", Name = "Send music wav file for recognition")]
        [ProducesResponseType<MusicDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<MusicDto>> RecognizeAudioFileDataAsync([FromForm]IFormFile audioFile, CancellationToken cancellationToken = default)
        {
            var dataBytes = await audioFile.ToBytes();
            var result =  await _recognitionService.RecognizeAudioAsync(dataBytes, cancellationToken);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result.ToDto());
        }
    }
}
