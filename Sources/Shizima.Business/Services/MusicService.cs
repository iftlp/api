using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private const String ERROR_PREFIX = "EXCEPTION.SERVICE.MUSIC.";

        /// <summary>
        /// The music service constructor
        /// </summary>
        /// <param name="unitOfWork">The database context instance</param>
        /// <param name="logger">The logger instance</param>
        public MusicService(UnitOfWork unitOfWork, ILogger<MusicService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(UnitOfWork)); ;
            _logger = logger;
        }

        /// <summary>
        /// Gets all musics.
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The list of musics</returns>
        public async Task<IEnumerable<Music>?> GetAllMusics(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting every musics entry.");
            return await _unitOfWork.DbContext.Musics.ToListAsync(cancellationToken);
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
            _logger.LogInformation("Success to create music {Id}", music.Id);
            return result;
        }
    }
}
