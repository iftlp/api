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
        /// Send music file data for recognition, as a user.
        /// </summary>
        /// <param name="audioFile">The music .wav file.</param>
        /// <param name="userToken">The user token that request the recognition</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The recognize music information.</returns>
        [HttpPost("corelib-wavFile/{userToken}", Name = "Send music file data for recognition, as a user")]
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
        /// Send music wav file for recognition.
        /// </summary>
        /// <param name="audioFile">The music .wav file.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The recognize music information.</returns>
        [HttpPost("corelib-wavFile", Name = "Send music wav file for recognition")]
        [ProducesResponseType<MusicDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<MusicDto>> RecognizeAudioFileDataAsync([FromForm]IFormFile audioFile, CancellationToken cancellationToken = default)
        {
            var tempFile = Path.GetTempFileName();
            await using (var stream = System.IO.File.Create(tempFile))
            {
                await audioFile.CopyToAsync(stream);
            }

            var dataBytes = await audioFile.ToBytes();
            var result =  await _recognitionService.RecognizeAudioAsync(dataBytes, cancellationToken);

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
        [HttpPost("wavFile/{userToken}", Name = "Route to send music file data for recognition, as a user. Using external lib")]
        [ProducesResponseType<HistoryDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<HistoryDto>> WithExtLib_RecognizeAudioFileDataUserAsync([FromForm] IFormFile audioFile, [FromRoute] string userToken, CancellationToken cancellationToken = default)
        {
            if (audioFile == null)
            {
                return new EmptyResult();
            }

            var tempFile = Path.GetTempFileName();

            if (tempFile == null)
            {
                return new EmptyResult();
            }

            await using (var stream = System.IO.File.Create(tempFile))
            {
                await audioFile.CopyToAsync(stream);
            }

            var result = await _recognitionService.WithExtLib_RecognizeAudioAsUserAsync(tempFile, userToken, cancellationToken);

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
        [HttpPost("wavFile", Name = "Route to send music wav file for recognition. Using external lib")]
        [ProducesResponseType<MusicDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<MusicDto>> WithExtLib_RecognizeAudioFileDataAsync([FromForm] IFormFile audioFile, CancellationToken cancellationToken = default)
        {
            if (audioFile == null)
            {
                return new EmptyResult();
            }
            
            var tempFile = Path.GetTempFileName();
            
            if (tempFile == null)
            {
                return new EmptyResult();
            }
            
            await using (var stream = System.IO.File.Create(tempFile))
            {
                await audioFile.CopyToAsync(stream);
            }

            var result = await _recognitionService.WithExtLib_RecognizeAudioAsync(tempFile, cancellationToken);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result.ToDto());
        }
    }
}
