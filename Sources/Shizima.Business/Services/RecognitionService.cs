using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Business.Extensions.Mappers;
using Shizima.CoreLib;
using Shizima.Entities;
using Shizima.Providers;
using SoundFingerprinting.Audio;
using SoundFingerprinting.Builder;
using SoundFingerprinting.Data;
using SoundFingerprinting.InMemory;
using System.Text.Json;
using NAudio.Wave;
using SoundFingerprinting.Configuration;

namespace Shizima.Business.Services
{
    public class RecognitionService
    {
        private readonly IAudioService _audioService = new SoundFingerprintingAudioService();

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

        /// <summary>
        /// Recognize audio data for a user.
        /// </summary>
        /// <param name="queryAudioData"></param>
        /// <param name="userToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The music information</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<History?> WithExtLib_RecognizeAudioAsUserAsync(string queryAudioDataFilePath, string userToken, CancellationToken cancellationToken)
        {
            using (var reader = new WaveFileReader(queryAudioDataFilePath))
            {
                int sampleRate = reader.WaveFormat.SampleRate;
                Console.WriteLine($"Sample rate: {sampleRate} Hz");
                _logger.LogError($"LOG RECOGNITION : Sample rate= {sampleRate} Hz.");
            }
            try
            {
                // Create an in-memory model service to hold the fingerprints
                var modelService = new InMemoryModelService();

                var avHashes = await FingerprintCommandBuilder.Instance
                                    .BuildFingerprintCommand()
                                    .From(queryAudioDataFilePath)
                                    .WithFingerprintConfig(config =>
                                    {
                                        config.Audio = new DefaultFingerprintConfiguration();
                                        return config;
                                    })
                                    .UsingServices(_audioService)
                                    .Hash();

                // Retrieve all fingerprints from the database
                var tempDatabaseFingerprints = await _unitOfWork.DbContext.JsonFingerprints.ToListAsync(cancellationToken);

                if (tempDatabaseFingerprints == null || tempDatabaseFingerprints.Count == 0)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No fingerprints found in the database.");
                    throw new ArgumentException($"No fingerprints found in the database.");
                }

                foreach (var item in tempDatabaseFingerprints)
                {
                    var fingerprints = AVHashesSerializer.Deserialize(item.Hash);
                    if (fingerprints == null)
                    {
                        _logger.LogError(ERROR_PREFIX + "RECOGNIZE: Fingerprint is null.");
                    }
                    else
                    {
                        var track = new TrackInfo(item.MusicId.ToString(), "", "");
                        modelService.Insert(track, fingerprints);
                    }
                }

                // Query the model service for the best match
                var recognitionResult = await QueryCommandBuilder.Instance
                                    .BuildQueryCommand()
                                    .From(queryAudioDataFilePath)
                                    .WithQueryConfig(config =>
                                    {
                                        config.Audio.FingerprintConfiguration = new DefaultFingerprintConfiguration();
                                        return config;
                                    })
                                    .UsingServices(modelService, _audioService)
                                    .Query();

                // Retrieve the music information based on the best match track ID
                if (recognitionResult == null || !recognitionResult.ContainsMatches)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No match in audio model.");
                    return null;
                }

                var musicResult = await _unitOfWork.DbContext.Musics
                                .Where(m => m.Id.ToString() == recognitionResult.BestMatch.TrackId)
                                .Include(m => m.Album.Artist)
                                .Include(m => m.Artists)
                                .FirstOrDefaultAsync(cancellationToken);

                if (musicResult == null)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: Music not found.");
                    return null;
                }

                // Check if the user exists, if not create a new user
                var user = await _unitOfWork.DbContext.Users.Where(u => u.Token == userToken).FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    await _userService.CreateUserAsync(userToken, cancellationToken);
                }

                user = await _unitOfWork.DbContext.Users.Where(u => u.Token == userToken).FirstOrDefaultAsync(cancellationToken);

                // Create a new history entry for the recognized music
                var history = new History()
                {
                    RecognitionDate = DateTime.UtcNow,
                    UserId = user!.Id,
                    User = user,
                    MusicId = musicResult.Id,
                    Music = musicResult
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

        public async Task<Music?> WithExtLib_RecognizeAudioAsync(string queryAudioDataFilePath, CancellationToken cancellationToken)
        {
             using (var reader = new WaveFileReader(queryAudioDataFilePath))
            {
                int sampleRate = reader.WaveFormat.SampleRate;
                Console.WriteLine($"Sample rate: {sampleRate} Hz");
                _logger.LogError($"LOG RECOGNITION : Sample rate= {sampleRate} Hz.");
            }
            try
            {
                // Create an in-memory model service to hold the fingerprints
                var modelService = new InMemoryModelService();

                var avHashes = await FingerprintCommandBuilder.Instance
                    .BuildFingerprintCommand()
                    .From(queryAudioDataFilePath)
                    .WithFingerprintConfig(config =>
                    {
                        config.Audio = new DefaultFingerprintConfiguration();
                        return config;
                    })
                    .UsingServices(_audioService)
                    .Hash();

                // Retrieve all fingerprints from the database
                var tempDatabaseFingerprints = await _unitOfWork.DbContext.JsonFingerprints.ToListAsync(cancellationToken);

                if (tempDatabaseFingerprints == null || tempDatabaseFingerprints.Count == 0)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No fingerprints found in the database.");
                    throw new ArgumentException($"No fingerprints found in the database.");
                }

                foreach (var item in tempDatabaseFingerprints)
                {
                    var fingerprints = AVHashesSerializer.Deserialize(item.Hash);
                    if (fingerprints == null)
                    {
                        _logger.LogError(ERROR_PREFIX + "RECOGNIZE: Fingerprint is null.");
                    }
                    else
                    {
                        var track = new TrackInfo(item.MusicId.ToString(), "", "");
                        modelService.Insert(track, fingerprints);
                    }
                }

                // Query the model service for the best match
                var recognitionResult = await QueryCommandBuilder.Instance
                    .BuildQueryCommand()
                    .From(queryAudioDataFilePath)
                    .WithQueryConfig(config =>
                    {
                        config.Audio.FingerprintConfiguration = new DefaultFingerprintConfiguration();
                        return config;
                    })
                    .UsingServices(modelService, _audioService)
                    .Query();

                if (recognitionResult == null || recognitionResult.BestMatch == null)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No match in audio model.");
                    return null;
                }

                // Retrieve the music information based on the best match track ID
                var musicResult = await _unitOfWork.DbContext.Musics
                    .Where(m => m.Id.ToString() == recognitionResult.BestMatch.TrackId)
                    .Include(m => m.Album.Artist)
                    .Include(m => m.Artists)
                    .FirstOrDefaultAsync(cancellationToken);

                if (musicResult == null)
                {
                    _logger.LogError(ERROR_PREFIX + "RECOGNIZE: No music found in the database.");
                    throw new ArgumentException($"No music found in the database.");
                }

                return musicResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ERROR_PREFIX + "RECOGNIZE: " + ex.Message);
                throw new ArgumentException($"{ERROR_PREFIX}RECOGNIZE: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate new audio fingerprints for a music using a new audio data file path.
        /// </summary>
        /// <param name="musicId">The music id</param>
        /// <param name="newAudioDataFilePath">The new audio file</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task WithExtLib_GenerateAudioFingerprintsAsync(long musicId, string newAudioDataFilePath, CancellationToken cancellationToken)
        {
            using (var reader = new WaveFileReader(newAudioDataFilePath))
            {
                int sampleRate = reader.WaveFormat.SampleRate;
                Console.WriteLine($"Sample rate: {sampleRate} Hz");
                _logger.LogError($"LOG RECOGNITION : Sample rate= {sampleRate} Hz.");
            }

            var rawFingerpints = await FingerprintCommandBuilder.Instance
                                .BuildFingerprintCommand()
                                .From(newAudioDataFilePath)
                                .WithFingerprintConfig(config =>
                                {
                                    config.Audio = new DefaultFingerprintConfiguration();
                                    return config;
                                })
                                .UsingServices(_audioService)
                                .Hash();
            
            var hashes = JsonSerializer.Serialize(rawFingerpints);

            var newDatabaseFingerprints = new List<JsonFingerprint>
            {
                new JsonFingerprint
                {
                    MusicId = musicId,
                    Hash = hashes
                }
            };

            await _unitOfWork.DbContext.AddRangeAsync(newDatabaseFingerprints, cancellationToken);
            _unitOfWork.SaveChanges();
        }
    }
}
