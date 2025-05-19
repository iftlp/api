using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Services;
using Shizima.WebAPI.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthController : Controller
    {
        private readonly AdministratorService _adminService;

        private readonly TokenProvider _tokenProvider;

        public AuthController(AdministratorService adminService, TokenProvider tokenProvider) 
        {
            _adminService = adminService;
            _tokenProvider = tokenProvider;
        }

        /// <summary>
        /// Get a Jwt token if the credentials are registered as admin.
        /// </summary>
        /// <param name="userIdentity">The administrator credentials (email/password).</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The Bearer token and it's expiration datetime (UTC)</returns>
        [HttpPost]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetTokenAsync([FromBody]UserIdentity userIdentity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _adminService.IsCredentialsAdminAsync(userIdentity.Email, userIdentity.Password, cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
            var response = new TokenResponse
            {
                Token = _tokenProvider.GenerateJwtToken(userIdentity.Email),
                Expires = DateTime.UtcNow.AddDays(1),
            };
             
            return new OkObjectResult(response);
        }

        /// <summary>
        /// Refresh a Jwt token if the actuel token is still avalaible.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The Bearer token and it's expiration datetime (UTC)</returns>
        [Authorize]
        [HttpGet("refresh")]
        public ActionResult<string> RefreshToken(CancellationToken cancellationToken)
        {
            var userIdentityEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                               ?? User.FindFirst("sub")?.Value;

            if (userIdentityEmail == null)
            {
                return new UnauthorizedObjectResult("Token is not valid.");
            }

            var response = new TokenResponse
            {
                Token = _tokenProvider.GenerateJwtToken(userIdentityEmail),
                Expires = DateTime.UtcNow.AddDays(1),
            };

            return new OkObjectResult(response);
        }
    }
}
