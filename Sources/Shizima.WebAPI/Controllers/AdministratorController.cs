using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Services;
using Shizima.WebAPI.Authentication;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("admin")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AdministratorController : Controller
    {
        private readonly AdministratorService _adminService;

        public AdministratorController(AdministratorService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Create a new administrator account.
        /// </summary>
        /// <param name="userIdentity">The new administrator credentials (email/password).</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The Bearer token and it's expiration datetime (UTC)</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateAdministrator([FromBody] UserIdentity userIdentity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _adminService.CreateAdministrator(userIdentity.Email, userIdentity.Password, cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }
    }
}
