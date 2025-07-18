using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.Services.BattleDefinitions;
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

        public BattleDefinitionsController(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IRewardsService rewardsService)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserBattleDefinitions()
        {
            var userId = User.GetId();
            var battles = await BattleDefinitionsService.GetActiveBattleDefinitionsByUserAsync(userId);
            var rewards = await RewardsService.GetRewardsFromBattleDefinitions(battles.Select(x => x.Id).ToArray());
            var battlesResources = battles.Select(x => new BattleDefinitionResource(x, rewards[x.Id]));
            return Ok(battlesResources);
        }
    }
}