using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Business.Extensions.Collections;
using Shizima.Entities;
using Shizima.Providers;

namespace Shizima.Business.Services
{
    public class MusicService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<MusicService> _logger;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Error prefix for the service
        /// </summary>
        private const string ERROR_PREFIX = "EXCEPTION.SERVICE.MUSIC.";

        /// <summary>
        /// The music service constructor
        /// </summary>
        /// <param name="unitOfWork">The database context instance</param>
        /// <param name="logger">The logger instance</param>
        public MusicService(UnitOfWork unitOfWork, ILogger<MusicService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }
        
        /// <summary>
        /// Gets music by id.
        /// </summary>
        /// <param name="id">The music id.</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The music</returns>
        public async Task<Music> GetMusicByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get music for id: {Id}.", id);
            var music = await _unitOfWork.DbContext.Musics
                .Where(m => m.Id == id)
                .Include(m => m.Album)
                .Include(m => m.Artists)
                .FirstOrDefaultAsync(cancellationToken);
            if (music == null)
            {
                _logger.LogError(ERROR_PREFIX + "GET_BY_ID: Music with id: {Id} was not found.", id);
                throw new ArgumentNullException($"{ERROR_PREFIX}GET_BY_ID: Music with id: {id} was not found.");
            }

            return music;
        }
        
        /// <summary>
        /// Gets all musics paginated.
        /// </summary>
        /// <param name="pageIndex">The index of the page</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The list of musics</returns>
        public async Task<PaginatedList<Music>?> GetAllMusicsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting every musics entry.");
            var musics = await _unitOfWork.DbContext.Musics
                .Include(m => m.Album)
                .Include(m => m.Artists)
                .ToListAsync(cancellationToken);
            return musics.ToPaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Create a music.
        /// </summary>
        /// <param name="music">The music entity</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task CreateMusicAsync(Music music, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to create music {Id}", music.Id);
            await _unitOfWork.DbContext.Musics.AddAsync(music, cancellationToken);
            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "CREATION: The music creation for {Id} failed.", music.Id);
                throw new ArgumentException($"{ERROR_PREFIX}CREATION: The music creation for {music.Id} failed.");
            }
            _logger.LogInformation("Success to create music {Id}", music.Id);
        }
        
        /// <summary>
        /// Update a music.
        /// </summary>
        /// <param name="id">The music id to update</param>
        /// <param name="music">The music entity new values</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task UpdateMusicAsync(int id, Music music, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to update music {Id}", id);
            var oldMusic = await _unitOfWork.DbContext.Musics.Where(m => m.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (oldMusic == null)
            {
                _logger.LogError(ERROR_PREFIX + "UPDATE: The music with id: {Id} does not exist.", id);
                throw new ArgumentException($"{ERROR_PREFIX}UPDATE: The music with id: {id} does not exist.");
            }
            
            oldMusic.Title = music.Title;
            oldMusic.Album = music.Album;
            oldMusic.Artists = music.Artists;

            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "UPDATE: The music update for {Id} failed.", music.Id);
                throw new ArgumentException($"{ERROR_PREFIX}UPDATE: The music update for {music.Id} failed.");
            }
            _logger.LogInformation("Success to update music {Id}", music.Id);
        }

        /// <summary>
        /// Delete a music.
        /// </summary>
        /// <param name="id">The music id to delete</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task DeleteMusicAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to update music {Id}", id);
            var musicToDelete = await _unitOfWork.DbContext.Musics.Where(m => m.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (musicToDelete == null)
            {
                _logger.LogError(ERROR_PREFIX + "DELETE: The music with id: {Id} does not exist.", id);
                throw new ArgumentException($"{ERROR_PREFIX}DELETE: The music with id: {id} does not exist.");
            }
            await _unitOfWork.DbContext.Musics.Where(m => m.Id == id).ExecuteDeleteAsync(cancellationToken);
            await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
