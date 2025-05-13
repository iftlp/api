using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;
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
        [Authorize]
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