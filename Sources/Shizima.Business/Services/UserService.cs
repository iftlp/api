using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Entities;
using Shizima.Providers;

namespace Shizima.Business.Services
{
    public class UserService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Error prefix for the service
        /// </summary>
        private const string ERROR_PREFIX = "EXCEPTION.SERVICE.USER.";

        /// <summary>
        /// The user service constructor
        /// </summary>
        /// <param name="unitOfWork">The database context instance</param>
        /// <param name="logger">The logger instance</param>
        public UserService(UnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="userToken">The user token</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The number of entry created (1 or 0)</returns>
        public async Task CreateUserAsync(string userToken, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Check if a user with this token already exist : {UserToken}", userToken);
            var userAlreadyPresent = await _unitOfWork.DbContext.Users.Where(u => u.Token == userToken).FirstOrDefaultAsync(cancellationToken);
            if (userAlreadyPresent != null)
            {
                _logger.LogError(ERROR_PREFIX + "CREATION: The user token: {UserToken} is already used.", userToken);
                throw new ArgumentException($"{ERROR_PREFIX}CREATION: The user token: {userToken} is already used");
            }

            _logger.LogInformation("Token not used. Try to create user {UserToken}", userToken);
            var user = new User
            {
                Token = userToken
            };

            await _unitOfWork.DbContext.AddAsync(user, cancellationToken);
            var result = await _unitOfWork.DbContext.SaveChangesAsync(cancellationToken);
            if (result == 0)
            {
                _logger.LogError(ERROR_PREFIX + "CREATION: The user creation for {UserToken} failed.", userToken);
                throw new ArgumentException($"{ERROR_PREFIX}CREATION: The user creation for {userToken} failed.");
            }
            _logger.LogInformation("Success to create user {UserToken}", userToken);
        }

        /// <summary>
        /// Get the history of a user.
        /// </summary>
        /// <param name="userToken">The user token</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>The history</returns>
        public async Task<IEnumerable<History>> GetUserHistoryAsync(string userToken, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Try to get user history for {UserToken}", userToken);
            
            var user = await _unitOfWork.DbContext.Users.Where(u => u.Token == userToken).FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                _logger.LogError(ERROR_PREFIX + "GET_HISTORY: The user with token: {UserToken} doesn't exist.", userToken);
                
                await CreateUserAsync(userToken, cancellationToken);
                
                user = await _unitOfWork.DbContext.Users.Where(u => u.Token == userToken).FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    _logger.LogError(ERROR_PREFIX + "GET_HISTORY: The user with token: {UserToken} failed to create.", userToken);
                }
            }
            
            var history = await _unitOfWork.DbContext.Histories
                .Where(h => h.UserId == user.Id)
                .Include(h => h.Music)
                .ThenInclude(m => m.Album)
                .ThenInclude(a => a.Artist)
                .ToListAsync(cancellationToken);

            if (history == null)
            {
                return Enumerable.Empty<History>();
            }
            
            return history;
        }
    }
}
