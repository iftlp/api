using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Extensions.Collections;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;
using Shizima.WebAPI.Extentions.Controller;
using System.ComponentModel;
using Shizima.Entities;
using Shizima.ClientAPI.MinioClient;
using Shizima.WebAPI.Extentions.Mappers;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("music")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MusicController : Controller
    {
        private readonly MusicService _musicService;
        private readonly RecognitionService _recognitionService;
        private readonly MinioService _minioService;
        private const string MINIO_FOLDER_NAME = "database";
        public MusicController(MusicService musicService, MinioService minioService, RecognitionService recognitionService) 
        {
            _musicService = musicService;
            _recognitionService = recognitionService;
            _minioService = minioService;
        }

        /// <summary>
        /// Get a music by id.
        /// </summary>
        /// <param name="id">The music id</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The music</returns>
        [HttpGet("{id:int}", Name = "Get a music by id")]
        [ProducesResponseType<MusicDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<MusicDto>> GetMusicByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Music music;
            try
            {
                music = await _musicService.GetMusicByIdAsync(id, cancellationToken);
            }
            catch (ArgumentNullException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkObjectResult(music.ToDto());
        }
        
        /// <summary>
        /// Get all the musics stored in database.
        /// </summary>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The musics entries.</returns>
        [Authorize]
        [HttpGet("all", Name = "Get all music entries of the database.")]
        [ProducesResponseType<IEnumerable<MusicDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<IEnumerable<MusicDto>>(StatusCodes.Status204NoContent)]
        [ProducesResponseType<IEnumerable<MusicDto>>(StatusCodes.Status206PartialContent)]
        [ProducesResponseType<IEnumerable<MusicDto>>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MusicDto>>> GetAllMusicsAsync([DefaultValue(0), FromQuery] int pageIndex = 0, [DefaultValue(20), FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var musics = await _musicService.GetAllMusicsAsync(pageIndex, pageSize, cancellationToken);

            return musics == null ? new NotFoundResult() : new ObjectResult(this.PaginatedResult(musics.MapTo(MusicMapper.ToDto)));
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
        public async Task<ActionResult> CreateMusicAsync([FromForm]MusicUploadRequestDto musicUpload, CancellationToken cancellationToken = default)
        {
            try
            {
                await _minioService.UploadFileAsync("shizima", MINIO_FOLDER_NAME + "/" + musicUpload.File.FileName, musicUpload.File);
                var dataBytes = await musicUpload.File.ToBytes();
                var musicId = await _musicService.CreateMusicAsync(musicUpload.Music.ToEntity(MINIO_FOLDER_NAME + "/" + musicUpload.File.FileName), cancellationToken);
                await _recognitionService.GenerateAudioFingerprintsAsync(musicId, dataBytes, cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
            return new OkResult();
        }

        /// <summary>
        /// Update a music entry of the database.
        /// </summary>
        /// <param name="id">The id of the music to update</param>
        /// <param name="musicDto">The music information</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id:int}", Name = "Update a music entry of the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateMusicAsync([FromRoute]int id, [FromBody] MusicDto musicDto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _musicService.UpdateMusicAsync(id, musicDto.ToEntity(), cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            return new OkResult();
        }

        /// <summary>
        /// Delete a music entry of the database.
        /// </summary>
        /// <param name="id">The id of the music to delete</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id:int}", Name = "Delete a music entry of the database.")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteMusicAsync([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _musicService.DeleteMusicAsync(id, cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
            return new OkResult();
        }
    }
}
