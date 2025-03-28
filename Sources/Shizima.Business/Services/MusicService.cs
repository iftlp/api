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
        /// Gets all musics.
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The list of musics</returns>
        public async Task<IEnumerable<Music>?> GetAllMusicsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting every musics entry.");
            return await _unitOfWork.DbContext.Musics.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets all musics paginated.
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The list of musics</returns>
        public async Task<PaginatedList<Music>?> GetAllMusicsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting every musics entry.");
            var musics = await _unitOfWork.DbContext.Musics.ToListAsync(cancellationToken);
            return musics.ToPaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Create a music.
        /// </summary>
        /// <param name="music">The music entity</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The number of entry created (1 or 0)</returns>
        public async Task<int> CreateMusicAsync(Music music, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to create music {Id}", music.Id);
            await _unitOfWork.DbContext.AddAsync(music, cancellationToken);
            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "CREATION: The music creation for {Id} failed.", music.Id);
                throw new ArgumentException($"{ERROR_PREFIX}.CREATION: The music creation for {music.Id} failed.");
            }
            _logger.LogInformation("Success to create music {Id}", music.Id);
            return result;
        }
    }
}
