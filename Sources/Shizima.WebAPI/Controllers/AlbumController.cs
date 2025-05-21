using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.ClientAPI.MinioClient;
using Shizima.Dtos;
using Shizima.Entities;
using Shizima.WebAPI.Extentions.Uploader;
using System.Text.RegularExpressions;

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
        private readonly MinioService _minioService;
        private const string MINIO_COVER_FOLDER_NAME = "database/cover/";
        public AlbumController(AlbumService albumService, MinioService minioService) 
        {
            _albumService = albumService;
            _minioService = minioService;
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
        /// Get albums by artist id.
        /// </summary>
        /// <param name="artistId">The artist id</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The list of albums</returns>
        [HttpGet("artist-id/{artistId:int}", Name = "Get albums by artist id")]
        [ProducesResponseType<IEnumerable<AlbumDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByArtistIdAsync(int artistId, CancellationToken cancellationToken = default)
        {
            IEnumerable<Album> albums;
            try
            {
                albums = await _albumService.GetAlbumsByArtistIdAsync(artistId, cancellationToken);
            }
            catch (ArgumentNullException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkObjectResult(albums.ToDto());
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
        public async Task<ActionResult> CreateAlbumAsync([FromForm]AlbumUploadRequestDto albumUpload, CancellationToken cancellationToken = default)
        {
            try
            {
                var coverSlug = Regex.Replace(albumUpload.Album.Title.ToLowerInvariant().Trim(), @"\s+", "-");
                var coverFileName = MINIO_COVER_FOLDER_NAME + coverSlug + ".png";

                await _albumService.CreateAlbumAsync(albumUpload.Album.ToEntity(coverFileName), cancellationToken);
                await _minioService.UploadFileAsync("shizima", coverFileName, albumUpload.Cover);
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
        /// <param name="albumUpload">The album information</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id:int}", Name = "Update an album entry of the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateAlbumAsync([FromRoute]int id, [FromForm]AlbumUploadRequestDto albumUpload, CancellationToken cancellationToken = default)
        {
            try
            {
                var coverSlug = Regex.Replace(albumUpload.Album.Title.ToLowerInvariant().Trim(), @"\s+", "-");
                var coverFileName = MINIO_COVER_FOLDER_NAME + coverSlug + ".png";

                await _albumService.UpdateAlbumAsync(id, albumUpload.Album.ToEntity(coverFileName), cancellationToken);
                await _minioService.UploadFileAsync("shizima", coverFileName, albumUpload.Cover);
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