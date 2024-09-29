using System;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Data.Exceptions;
using Epic.Server.Authentication;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/user/units")]
    public class UserUnitsController : ControllerBase
    {
        [NotNull] public IUserUnitsService UserUnitsService { get; }

        public UserUnitsController([NotNull] IUserUnitsService userUnitsService)
        {
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserUnits()
        {
            try
            {
                var units = await UserUnitsService.GetAliveUnitsByUserAsync(User.GetId());
                return Ok(units);
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }
    }
}