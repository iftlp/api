using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Services;
using Shizima.WebAPI.Authentication;

namespace Shizima.WebAPI.Controllers
{
    [ApiController]
    [Route("user")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService, TokenProvider tokenProvider) 
        {
            _userService = userService;
        }

        /// <summary>
        /// Create a new user with token.
        /// </summary>
        /// <param name="userToken">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost("{userToken}")]
        [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateUser(string userToken, CancellationToken cancellationToken = default)
        {
            var result = await _userService.CreateUserAsync(userToken, cancellationToken);
            return result == 0 ? new BadRequestResult() : new OkResult();
        }
    }
}
