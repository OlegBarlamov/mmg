using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Players;
using Epic.Core.Services.Units;
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
        [NotNull] public IPlayerUnitsService PlayerUnitsService { get; }
        public IPlayersService PlayersService { get; }

        public UserUnitsController([NotNull] IPlayerUnitsService playerUnitsService, [NotNull] IPlayersService playersService)
        {
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
        }

        [HttpGet("army")]
        public async Task<IActionResult> GetArmyUnits()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            try
            {
                var player = await PlayersService.GetById(playerId);
                var armyUnits = await PlayerUnitsService.GetAliveUnitsByContainerId(player.ArmyContainerId);
                return Ok(armyUnits.Select(x => new UserUnitInDashboardResource(x)));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }
        
        [HttpGet("supply")]
        public async Task<IActionResult> GetSupplyUnits()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            try
            {
                var player = await PlayersService.GetById(playerId);
                var supplyUnits = await PlayerUnitsService.GetAliveUnitsByContainerId(player.SupplyContainerId);
                return Ok(supplyUnits.Select(x => new UserUnitInDashboardResource(x)));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }
    }
}