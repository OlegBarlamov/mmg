using System;
using System.Threading.Tasks;
using Epic.Core.Services.Players;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.UnitsContainers.Errors;
using Epic.Data.Exceptions;
using Epic.Server.Authentication;
using Epic.Server.RequestBodies;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/units")]
    public class UserUnitsController : ControllerBase
    {
        [NotNull] public IPlayerUnitsService PlayerUnitsService { get; }
        public IPlayersService PlayersService { get; }
        public IContainersManipulator ContainersManipulator { get; }

        public UserUnitsController(
            [NotNull] IPlayerUnitsService playerUnitsService,
            [NotNull] IPlayersService playersService,
            [NotNull] IContainersManipulator containersManipulator)
        {
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            ContainersManipulator = containersManipulator ?? throw new ArgumentNullException(nameof(containersManipulator));
        }

        [HttpGet("containers/{containerId}")]
        public async Task<IActionResult> GetUnitsContainer(Guid containerId)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            try
            {
                var container = await ContainersManipulator.GetContainerWithUnits(containerId);
                if (container.OwnerPlayerId != playerId)
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        message = "You are not authorized to see this container."
                    });
                
                return Ok(new UnitsContainerWithUnitsResource(container));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
        }

        [HttpPost("move/{unitId}")]
        public async Task<IActionResult> MoveUnits(Guid unitId, [FromBody] ManipulateContainerUnitsRequestBody requestBody)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);

            var targetUnit = await PlayerUnitsService.GetById(unitId);
            if (targetUnit.PlayerId != playerId)
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not authorized to manipulate this unit."
                });

            try
            {
                var containerWithUnits = await ContainersManipulator.MoveUnits(
                    targetUnit,
                    requestBody.ContainerId ?? targetUnit.ContainerId,
                    requestBody.Amount ?? targetUnit.Count,
                    requestBody.SlotIndex);

                return Ok(new UnitsContainerWithUnitsResource(containerWithUnits));
            }
            catch (InvalidUnitSlotsOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}