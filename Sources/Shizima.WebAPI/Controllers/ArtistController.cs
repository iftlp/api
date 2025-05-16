using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Extensions.Collections;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;
using Shizima.WebAPI.Extentions.Controller;
using System.ComponentModel;
using Shizima.Entities;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("artist")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class ArtistController : Controller
    {
        
        private readonly ArtistService _artistService;

        public ArtistController(ArtistService artistService) 
        {
            _artistService = artistService;
        }

        /// <summary>
        /// Get an artist by id.
        /// </summary>
        /// <param name="id">The artist id</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The artist</returns>
        [HttpGet("{id:int}", Name = "Get an artist by id")]
        [ProducesResponseType<ArtistDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<ArtistDto>> GetArtistByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Artist artist;
            try
            {
                artist = await _artistService.GetArtistByIdAsync(id, cancellationToken);
            }
            catch (ArgumentNullException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkObjectResult(artist.ToDto());
        }

        /// <summary>
        /// Get an artist by music id.
        /// </summary>
        /// <param name="musicId">The music id</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The list of artists</returns>
        [HttpGet("music-id/{musicId:int}", Name = "Get an artist by music id")]
        [ProducesResponseType<IEnumerable<ArtistDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtistByMusicIdAsync(int musicId, CancellationToken cancellationToken = default)
        {
            IEnumerable<Artist> artist;
            try
            {
                artist = await _artistService.GetArtistByMusicIdAsync(musicId, cancellationToken);
            }
            catch (ArgumentNullException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkObjectResult(artist.ToDto());
        }

        /// <summary>
        /// Get all the artists stored in database.
        /// </summary>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The artists entries.</returns>
        [Authorize]
        [HttpGet("all", Name = "Get all artist entries of the database.")]
        [ProducesResponseType<IEnumerable<ArtistDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<IEnumerable<ArtistDto>>(StatusCodes.Status204NoContent)]
        [ProducesResponseType<IEnumerable<ArtistDto>>(StatusCodes.Status206PartialContent)]
        [ProducesResponseType<IEnumerable<ArtistDto>>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MusicDto>>> GetAllMusicsAsync([DefaultValue(0), FromQuery] int pageIndex = 0, [DefaultValue(20), FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var artists = await _artistService.GetAllArtistsAsync(pageIndex, pageSize, cancellationToken);

            return artists == null ? new NotFoundResult() : new ObjectResult(this.PaginatedResult(artists.MapTo(ArtistMapper.ToDto)));
        }

        /// <summary>
        /// Add an artist entry to the database.
        /// </summary>
        /// <param name="artistDto">The artist information</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(Name = "Add an artist entry to the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateArtistAsync([FromBody] ArtistDto artistDto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _artistService.CreateArtistAsync(artistDto.ToEntity(), cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }

        /// <summary>
        /// Update an artist entry of the database.
        /// </summary>
        /// <param name="id">The id of the artist to update</param>
        /// <param name="artistDto">The artist information</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id:int}", Name = "Update an artist entry of the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateArtistAsync([FromRoute] int id, [FromBody] ArtistDto artistDto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _artistService.UpdateArtistAsync(id, artistDto.ToEntity(), cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }

        /// <summary>
        /// Delete an artist entry of the database.
        /// </summary>
        /// <param name="id">The id of the artist to delete</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id:int}", Name = "Delete an artist entry of the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteArtistAsync([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _artistService.DeleteArtistAsync(id, cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }
    }
}