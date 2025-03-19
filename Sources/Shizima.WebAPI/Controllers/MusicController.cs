using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("music")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class MusicController
    {
        private readonly MusicService _musicService;

        public MusicController(MusicService musicService) 
        {
            _musicService = musicService;
        }

        /// <summary>
        /// Get all the musics stored in database.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The musics entries.</returns>
        [HttpGet("all", Name = "Get all music entries of the database.")]
        [ProducesResponseType<IEnumerable<MusicDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MusicDto>>> RecoguenizeMusic(CancellationToken cancellationToken = default)
        {
            var musics = await _musicService.GetAllMusics(cancellationToken);
            return musics == null ? new NotFoundResult() : new OkObjectResult(musics.ToDto());
        }
    }
}
