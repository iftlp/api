using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Entities;
using Shizima.Providers;

namespace Shizima.Business.Services
{
    public class AlbumService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<AlbumService> _logger;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Error prefix for the service
        /// </summary>
        private const string ERROR_PREFIX = "EXCEPTION.SERVICE.ALBUM.";

        /// <summary>
        /// The album service constructor
        /// </summary>
        /// <param name="unitOfWork">The database context instance</param>
        /// <param name="logger">The logger instance</param>
        public AlbumService(UnitOfWork unitOfWork, ILogger<AlbumService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }
        
        /// <summary>
        /// Get album by id.
        /// </summary>
        /// <param name="id">The id of the album</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The album</returns>
        public async Task<Album> GetAlbumByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to get album for id: {Id}", id);
            var album = await _unitOfWork.DbContext.Albums
                .Where(a => a.Id == id)
                .Include(a => a.Musics)
                    .ThenInclude(m => m.Artists!)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(cancellationToken);
            if (album == null)
            {
                _logger.LogError(ERROR_PREFIX + "GET_BY_ID: The album for id: {Id} not found.", id);
                throw new ArgumentException($"{ERROR_PREFIX}GET_BY_ID: The album for id: {id} not found.");
            }
            return album;
        }

        /// <summary>
        /// Get list of albums by artist id.
        /// </summary>
        /// <param name="artistId">The id of the artist</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The list of albums</returns>
        public async Task<IEnumerable<Album>> GetAlbumsByArtistIdAsync(int artistId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to get albums for artist id: {Id}", artistId);
            var albums = await _unitOfWork.DbContext.Albums.Where(a => a.ArtistId == artistId).ToListAsync(cancellationToken);
            if (albums == null)
            {
                _logger.LogError(ERROR_PREFIX + "GET_BY_ID: The albums for artist id: {Id} not found.", artistId);
                throw new ArgumentException($"{ERROR_PREFIX}GET_BY_ID: The albums for artist id: {artistId} not found.");
            }
            return albums;
        }

        /// <summary>
        /// Create an album.
        /// </summary>
        /// <param name="album">The album entity</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task CreateAlbumAsync(Album album, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to create album {Id}", album.Id);
            await _unitOfWork.DbContext.Albums.AddAsync(album, cancellationToken);
            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "CREATION: The album creation for {Id} failed.", album.Id);
                throw new ArgumentException($"{ERROR_PREFIX}CREATION: The album creation for {album.Id} failed.");
            }
            _logger.LogInformation("Success to create album {Id}", album.Id);
        }
        
        /// <summary>
        /// Update a album.
        /// </summary>
        /// <param name="id">The album id to update</param>
        /// <param name="album">The album entity new values</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task UpdateAlbumAsync(int id, Album album, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to update album {Id}", id);
            var oldAlbum = await _unitOfWork.DbContext.Albums.Where(a => a.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (oldAlbum == null)
            {
                _logger.LogError(ERROR_PREFIX + "UPDATE: The album with id: {Id} does not exist.", id);
                throw new ArgumentException($"{ERROR_PREFIX}UPDATE: The album with id: {id} does not exist.");
            }
            
            oldAlbum.Title = album.Title;
            oldAlbum.Cover = album.Cover;
            oldAlbum.Musics = album.Musics;

            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "UPDATE: The album update for {Id} failed.", album.Id);
                throw new ArgumentException($"{ERROR_PREFIX}UPDATE: The album update for {album.Id} failed.");
            }
            _logger.LogInformation("Success to update album {Id}", album.Id);
        }

        /// <summary>
        /// Delete a album.
        /// </summary>
        /// <param name="id">The album id to delete</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task DeleteAlbumAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to update album {Id}", id);
            var albumToDelete = await _unitOfWork.DbContext.Albums.Where(m => m.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (albumToDelete == null)
            {
                _logger.LogError(ERROR_PREFIX + "DELETE: The album with id: {Id} does not exist.", id);
                throw new ArgumentException($"{ERROR_PREFIX}DELETE: The album with id: {id} does not exist.");
            }
            await _unitOfWork.DbContext.Albums.Where(m => m.Id == id).ExecuteDeleteAsync(cancellationToken);
            await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}