using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Entities;
using Shizima.Providers;

namespace Shizima.Business.Services
{
    public class ArtistService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ArtistService> _logger;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Error prefix for the service
        /// </summary>
        private const string ERROR_PREFIX = "EXCEPTION.SERVICE.ARTIST.";

        /// <summary>
        /// The artist service constructor
        /// </summary>
        /// <param name="unitOfWork">The database context instance</param>
        /// <param name="logger">The logger instance</param>
        public ArtistService(UnitOfWork unitOfWork, ILogger<ArtistService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }
        
        /// <summary>
        /// Get artist by id.
        /// </summary>
        /// <param name="id">The id of the artist</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The artist</returns>
        public async Task<Artist> GetArtistByIdAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to get artist for id: {Id}", id);
            var artist = await _unitOfWork.DbContext.Artists.Where(a => a.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (artist == null)
            {
                _logger.LogError(ERROR_PREFIX + "GET_BY_ID: The artist for id: {Id} not found.", id);
                throw new ArgumentException($"{ERROR_PREFIX}GET_BY_ID: The artist for id: {id} not found.");
            }
            return artist;
        }
        
        /// <summary>
        /// Create an artist.
        /// </summary>
        /// <param name="artist">The artist entity</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task CreateArtistAsync(Artist artist, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to create artist {Id}", artist.Id);
            await _unitOfWork.DbContext.Artists.AddAsync(artist, cancellationToken);
            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "CREATION: The artist creation for {Id} failed.", artist.Id);
                throw new ArgumentException($"{ERROR_PREFIX}CREATION: The artist creation for {artist.Id} failed.");
            }
            _logger.LogInformation("Success to create artist {Id}", artist.Id);
        }
        
        /// <summary>
        /// Update a artist.
        /// </summary>
        /// <param name="id">The artist id to update</param>
        /// <param name="artist">The artist entity new values</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task UpdateArtistAsync(int id, Artist artist, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to update artist {Id}", id);
            var oldArtist = await _unitOfWork.DbContext.Artists.Where(a => a.Id == id)
                                                                //.Include(a => a.Musics)
                                                                //.Include(a => a.Musics!.Select(m => m.Album))
                                                                .FirstOrDefaultAsync(cancellationToken);
            if (oldArtist == null)
            {
                _logger.LogError(ERROR_PREFIX + "UPDATE: The artist with id: {Id} does not exist.", id);
                throw new ArgumentException($"{ERROR_PREFIX}UPDATE: The artist with id: {id} does not exist.");
            }
            
            oldArtist.Name = artist.Name;
            oldArtist.Musics = artist.Musics;

            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "UPDATE: The artist update for {Id} failed.", artist.Id);
                throw new ArgumentException($"{ERROR_PREFIX}UPDATE: The artist update for {artist.Id} failed.");
            }
            _logger.LogInformation("Success to update artist {Id}", artist.Id);
        }

        /// <summary>
        /// Delete a artist.
        /// </summary>
        /// <param name="id">The artist id to delete</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task DeleteArtistAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to update artist {Id}", id);
            var artistToDelete = await _unitOfWork.DbContext.Artists.Where(m => m.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (artistToDelete == null)
            {
                _logger.LogError(ERROR_PREFIX + "DELETE: The artist with id: {Id} does not exist.", id);
                throw new ArgumentException($"{ERROR_PREFIX}DELETE: The artist with id: {id} does not exist.");
            }
            await _unitOfWork.DbContext.Artists.Where(m => m.Id == id).ExecuteDeleteAsync(cancellationToken);
            await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}