using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;
using Shizima.Entities;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("album")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class AlbumController : Controller
    {
        
        private readonly AlbumService _albumService;

        public AlbumController(AlbumService albumService) 
        {
            _albumService = albumService;
        }

        /// <summary>
        /// Get an album by id.
        /// </summary>
        /// <param name="id">The album id</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The album</returns>
        [HttpGet("{id:int}", Name = "Get an album by id")]
        [ProducesResponseType<AlbumDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<AlbumDto>> GetAlbumByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Album album;
            try
            {
                album = await _albumService.GetAlbumByIdAsync(id, cancellationToken);
            }
            catch (ArgumentNullException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkObjectResult(album.ToDto());
        }
        
        /// <summary>
        /// Add an album entry to the database.
        /// </summary>
        /// <param name="albumDto">The album information</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(Name = "Add an album entry to the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateAlbumAsync([FromBody] AlbumDto albumDto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _albumService.CreateAlbumAsync(albumDto.ToEntity(), cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }

        /// <summary>
        /// Update an album entry of the database.
        /// </summary>
        /// <param name="id">The id of the album to update</param>
        /// <param name="albumDto">The album information</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id:int}", Name = "Update an album entry of the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateAlbumAsync([FromRoute] int id, [FromBody] AlbumDto albumDto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _albumService.UpdateAlbumAsync(id, albumDto.ToEntity(), cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }

        /// <summary>
        /// Delete an album entry of the database.
        /// </summary>
        /// <param name="id">The id of the album to delete</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id:int}", Name = "Delete an album entry of the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAlbumAsync([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _albumService.DeleteAlbumAsync(id, cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }
    }
}