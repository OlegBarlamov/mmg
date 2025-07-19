using System;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.Services.Users;
using Epic.Data.Exceptions;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        public IUsersService UsersService { get; }

        public UserController([NotNull] IUsersService usersService)
        {
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await UsersService.GetUserById(User.GetId());
                return Ok(new UserResource(user, User.GetPlayerId()));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }
    }
}