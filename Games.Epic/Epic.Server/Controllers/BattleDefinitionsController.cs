using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Server.Authentication;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/battles")]
    public class BattleDefinitionsController : ControllerBase
    {
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IRewardsService RewardsService { get; }
        public IPlayersService PlayersService { get; }

        public BattleDefinitionsController(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IRewardsService rewardsService,
            [NotNull] IPlayersService playersService)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserBattleDefinitions()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            var player = await PlayersService.GetById(playerId);
            var battles = await BattleDefinitionsService.GetNotExpiredActiveBattleDefinitionsByPlayerAsync(playerId);
            var rewards = await RewardsService.GetRewardsFromBattleDefinitions(battles.Select(x => x.Id).ToArray());
            var battlesResources = battles.Select(x => new BattleDefinitionResource(x, rewards[x.Id], player));
            return Ok(battlesResources);
        }
    }
}