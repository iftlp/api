using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Business.Extensions.Mappers;
using Shizima.CoreLib;
using Shizima.Dtos;
using Shizima.Entities;
using Shizima.Providers;

namespace Shizima.Business.Services
{
    public class RecognitionService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<RecognitionService> _logger;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Error prefix for the service
        /// </summary>
        private const string ERROR_PREFIX = "EXCEPTION.SERVICE.RECOGNITION.";

        /// <summary>
        /// The music service constructor
        /// </summary>
        /// <param name="unitOfWork">The database context instance</param>
        /// <param name="logger">The logger instance</param>
        public RecognitionService(UnitOfWork unitOfWork, ILogger<RecognitionService> logger, UserService userService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Recognize audio data for a user.
        /// </summary>
        /// <param name="queryAudioData"></param>
        /// <param name="userToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The music information</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<History?> RecognizeAudioAsUserAsync(IEnumerable<byte> queryAudioData, string userToken, CancellationToken cancellationToken)
        {
            try
            {
                var queryFingerprint = FingerprintGenerator.AnalyzeAudio(queryAudioData.ToArray());

                var tempDatabaseFingerprints = await _unitOfWork.DbContext.Fingerprints.ToListAsync(cancellationToken);

                if (tempDatabaseFingerprints == null || tempDatabaseFingerprints.Count == 0)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No fingerprints found in the database.");
                    throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: No fingerprints found in the database.");
                }

                var databaseFingerprints = tempDatabaseFingerprints!.ToDict();

                var resultList = AudioRecognizer.RecognizeAudioList(queryFingerprint, databaseFingerprints);

                if (resultList == null || resultList.Count == 0)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No music found.");
                    throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: No music found.");
                }

                var music = await _unitOfWork.DbContext.Musics.Where(m => m.Id == resultList.First()!.TrackId)
                    .Include(m => m.Album.Artist)
                    .Include(m => m.Artists)
                    .FirstOrDefaultAsync(cancellationToken);

                if (music == null)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: Music not found.");
                    throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: Music not found.");
                }

                var user = await _unitOfWork.DbContext.Users.Where(u => u.Token == userToken).FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    await _userService.CreateUserAsync(userToken, cancellationToken);
                }

                user = await _unitOfWork.DbContext.Users.Where(u => u.Token == userToken).FirstOrDefaultAsync(cancellationToken);

                var history = new History()
                {
                    RecognitionDate = DateTime.UtcNow,
                    UserId = user!.Id,
                    User = user,
                    MusicId = music.Id,
                    Music = music
                };

                var newHistoryEntry = await _unitOfWork.DbContext.AddAsync(history, cancellationToken);
                _unitOfWork.SaveChanges();

                return newHistoryEntry.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ERROR_PREFIX + "RECOGNIZE: " + ex.Message);
                throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: {ex.Message}");
            }
        }

        /// <summary>
        /// Recognize audio data.
        /// </summary>
        /// <param name="queryAudioData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Music?> RecognizeAudioAsync(IEnumerable<byte> queryAudioData, CancellationToken cancellationToken)
        {
            try
            {
                var queryFingerprint = FingerprintGenerator.AnalyzeAudio(queryAudioData.ToArray());

                var tempDatabaseFingerprints = await _unitOfWork.DbContext.Fingerprints.ToListAsync(cancellationToken);

                if (tempDatabaseFingerprints == null || tempDatabaseFingerprints.Count == 0)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No fingerprints found in the database.");
                    throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: No fingerprints found in the database.");
                }

                var databaseFingerprints = tempDatabaseFingerprints!.ToDict();

                var resultList = AudioRecognizer.RecognizeAudioList(queryFingerprint, databaseFingerprints);

                if (resultList == null || resultList.Count == 0)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No music found.");
                    throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: No music found.");
                }

                var music = await _unitOfWork.DbContext.Musics.Where(m => m.Id == resultList.First()!.TrackId)
                    .Include(m => m.Album)
                    .ThenInclude(a => a.Artist)
                    .Include(m => m.Artists)
                    .FirstOrDefaultAsync(cancellationToken);

                if (music == null)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: Music not found.");
                    throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: Music not found.");
                }

                return music;
            }
            catch (Exception ex)
            {
                _logger.LogError(ERROR_PREFIX + "RECOGNIZE: " + ex.Message);
                throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate audio fingerprints for a music.
        /// </summary>
        /// <param name="musicId"></param>
        /// <param name="newAudioData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GenerateAudioFingerprintsAsync(long musicId, IEnumerable<byte> newAudioData, CancellationToken cancellationToken)
        {
            var newFingerprints = FingerprintGenerator.AnalyzeAudio(newAudioData.ToArray());

            var newDatabaseFingerprints = newFingerprints.ToDatabase(musicId);

            await _unitOfWork.DbContext.AddRangeAsync(newDatabaseFingerprints, cancellationToken);
            _unitOfWork.SaveChanges();
        }
    }
}
