using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shizima.Entities;
using Shizima.Providers;
using System.Security.Cryptography;

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
        /// The number of iterations for the hash function
        /// </summary>
        private const int ITERATIONS = 10000;

        /// <summary>
        /// The size of the hash in bytes
        /// </summary>
        private const int HASH_SIZE = 32;

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
            var administrator = await _unitOfWork.DbContext.Administrators.Where(a => a.Email == email).FirstOrDefaultAsync(cancellationToken);
            
            if (administrator == null || administrator.Password == null)
            {
                _logger.LogError("No administrator exist in database with email {Email}, and is admin.", email);
                throw new ArgumentException($"{ERROR_PREFIX}AUTHENTICATION_FAILED: No administrator exist in database with email {email}.");
            }

            if (!VerifyPassword(password, administrator.Password))
            {
                _logger.LogError("Authentication failed: Invalid password for email '{Email}'.", email);
                throw new ArgumentException($"{ERROR_PREFIX}AUTHENTICATION_FAILED: Invalid password for administrator with email {email}.");
            }
        }

        /// <summary>
        /// Create a new administrator with the provided email and password.
        /// </summary>
        /// <param name="email">The administrator's email</param>
        /// <param name="password">The administrator's password</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        public async Task CreateAdministrator(string email, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("Admin creation: Missing or empty credentials. Email: '{Email}'.", email);
                throw new ArgumentException($"{ERROR_PREFIX}ADMIN_CREATION: Provided email or password is empty.");
            }
            _logger.LogInformation("Try create administrator with email: {Email}.", email);

            string hashedPassword = HashPassword(password);
            var administrator = new Administrator
            {
                Email = email,
                Password = hashedPassword
            };

            await _unitOfWork.DbContext.Administrators.AddAsync(administrator, cancellationToken);
            var result = _unitOfWork.SaveChanges();
            
            if (result == -1)
            {
                _logger.LogError("Admin creation: An account with email: '{Email}' already exist.", email);
                throw new ArgumentException($"{ERROR_PREFIX}ADMIN_CREATION: An account with email: {email} already exist.");
            }
        }

        /// <summary>
        /// Create a salt with the desired size.
        /// </summary>
        /// <param name="size">Salt size</param>
        /// <returns>The random salt</returns>
        private byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        /// <summary>
        /// Génère un hash avec PBKDF2
        /// </summary>
        /// <param name="password">Clear password</param>
        /// <param name="salt">The salt</param>
        /// <param name="iterations">Iteration number</param>
        /// <param name="hashSize">Hash size in bytes</param>
        /// <returns>The hash</returns>
        private byte[] GenerateHash(string password, byte[] salt, int iterations, int hashSize)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(hashSize);
            }
        }

        /// <summary>
        /// Hashes a password using PBKDF2 with HMAC-SHA256
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>The salt and hash combined in Base64 format</returns>
        private string HashPassword(string password)
        {
            byte[] salt = GenerateSalt();

            byte[] hash = GenerateHash(password, salt, ITERATIONS, HASH_SIZE);

            byte[] iterationBytes = BitConverter.GetBytes(ITERATIONS);
            byte[] hashBytes = new byte[salt.Length + iterationBytes.Length + hash.Length];

            Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
            Buffer.BlockCopy(iterationBytes, 0, hashBytes, salt.Length, iterationBytes.Length);
            Buffer.BlockCopy(hash, 0, hashBytes, salt.Length + iterationBytes.Length, hash.Length);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Compare two byte arrays in constant time to avoid timing attacks
        /// </summary>
        /// <param name="a">First array</param>
        /// <param name="b">Second array</param>
        /// <returns>True if the arrays matches, false otherwise</returns>
        private bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        /// <summary>
        /// Check if a plain text password matches the hashed version
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hashedPassword">The hashed password to compare against</param>
        /// <returns>True if the passwords matches, false otherwise</returns>
        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                byte[] salt = new byte[16];
                Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

                int iterations = BitConverter.ToInt32(hashBytes, 16);

                int hashSize = hashBytes.Length - (16 + 4);
                byte[] originalHash = new byte[hashSize];
                Buffer.BlockCopy(hashBytes, 16 + 4, originalHash, 0, hashSize);

                byte[] newHash = GenerateHash(password, salt, iterations, hashSize);

                return SlowEquals(originalHash, newHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
