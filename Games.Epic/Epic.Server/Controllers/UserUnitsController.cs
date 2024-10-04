using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Data.Exceptions;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/units")]
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
                return Ok(units.Select(x => new UserUnitInDashboardResource(x)));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }
    }
}