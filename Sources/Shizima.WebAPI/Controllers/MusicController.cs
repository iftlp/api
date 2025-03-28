using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Extensions.Collections;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;
using Shizima.WebAPI.Extentions.Controller;
using System.ComponentModel;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("music")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class MusicController : Controller
    {
        private readonly MusicService _musicService;

        public MusicController(MusicService musicService) 
        {
            _musicService = musicService;
        }

        /// <summary>
        /// Get all the musics stored in database.
        /// </summary>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The musics entries.</returns>
        [HttpGet("all", Name = "Get all music entries of the database.")]
        [ProducesResponseType<IEnumerable<MusicDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MusicDto>>> GetAllMusics([DefaultValue(0), FromQuery] int pageIndex = 0, [DefaultValue(20), FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var musics = await _musicService.GetAllMusicsAsync(pageIndex, pageSize, cancellationToken);

            return musics == null ? new NotFoundResult() : new OkObjectResult(this.PaginatedResult(musics.MapTo(MusicMapper.ToDto)));
        }

        /// <summary>
        /// Add a music entry to the database.
        /// </summary>
        /// <param name="musicDto">The music information</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(Name = "Add a music entry to the database.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddMusic(MusicDto musicDto, CancellationToken cancellationToken = default)
        {
            var result = await _musicService.CreateMusicAsync(musicDto.ToEntity(), cancellationToken);
            return result == 0 ? new BadRequestResult() : new OkResult();
        }
    }
}
