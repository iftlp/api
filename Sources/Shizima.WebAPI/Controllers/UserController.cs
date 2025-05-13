using Microsoft.AspNetCore.Mvc;
using Shizima.Business.Mappers;
using Shizima.Business.Services;
using Shizima.Dtos;
using Shizima.Entities;
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
        public async Task<ActionResult> CreateUserAsync([FromRoute]string userToken, CancellationToken cancellationToken = default)
        {
            try
            {
                await _userService.CreateUserAsync(userToken, cancellationToken);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
            return new OkResult();
        }

        /// <summary>
        /// Get the history from user token.
        /// </summary>
        /// <param name="userToken">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The history</returns>
        [HttpGet("{userToken}/history")]
        [ProducesResponseType<IEnumerable<HistoryDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HistoryDto>>> GetUserHistoryAsync(string userToken, CancellationToken cancellationToken = default)
        {
            IEnumerable<History> history;

            try
            {
                history = await _userService.GetUserHistoryAsync(userToken, cancellationToken);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkObjectResult(history.ToDto());
        }
    }
}
