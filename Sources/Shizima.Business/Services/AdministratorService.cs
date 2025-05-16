using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Entities;
using Shizima.Providers;

namespace Shizima.Business.Services
{
    public class AdministratorService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<AdministratorService> _logger;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Error prefix for the service
        /// </summary>
        private const string ERROR_PREFIX = "EXCEPTION.SERVICE.ADMINISTRATOR.";

        /// <summary>
        /// The administrator service constructor
        /// </summary>
        /// <param name="unitOfWork">The database context instance</param>
        /// <param name="logger">The logger instance</param>
        public AdministratorService(UnitOfWork unitOfWork, ILogger<AdministratorService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        /// <summary>
        /// Try to get a administrator by its login/password, and check if he is admin.
        /// </summary>
        /// <param name="email">The administrator's email</param>
        /// <param name="password">The administrator's password</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>True if a administrator exist. False otherwise.</returns>
        public async Task IsCredentialsAdminAsync(string email, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("Authentication failed: Missing or empty credentials. Email: '{Email}'.", email);
                throw new ArgumentException($"{ERROR_PREFIX}AUTHENTICATION_FAILED: Provided email or password is empty.");
            }

            _logger.LogInformation("Try get administrator from email: {Email}.", email);
            var administrator = await _unitOfWork.DbContext.Administrators.Where(a => a.Email == email &&
                                                                                    a.Password == password
                                                                                    ).FirstOrDefaultAsync(cancellationToken);
            if (administrator == null)
            {
                _logger.LogError("No administrator exist in database with email {Email}, and is admin.", email);
                throw new ArgumentException($"{ERROR_PREFIX}AUTHENTICATION_FAILED: No administrator exist in database with email {email}.");
            }
        }

        public async Task CreateAdministrator(string email, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("Authentication failed: Missing or empty credentials. Email: '{Email}'.", email);
                throw new ArgumentException($"{ERROR_PREFIX}AUTHENTICATION_FAILED: Provided email or password is empty.");
            }
            _logger.LogInformation("Try create administrator with email: {Email}.", email);
            var administrator = new Administrator
            {
                Email = email,
                Password = password
            };

            await _unitOfWork.DbContext.Administrators.AddAsync(administrator, cancellationToken);
            _unitOfWork.SaveChanges();
        }
    }
}
