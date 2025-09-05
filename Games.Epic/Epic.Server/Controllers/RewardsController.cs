using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.Reward;
using Epic.Server.Authentication;
using Epic.Server.RequestBodies;
using Epic.Server.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Epic.Server.Controllers
{
    [ApiController]
    [Route("api/rewards")]
    public class RewardsController : ControllerBase
    {
        private IRewardsService RewardsService { get; }
        public IUnitTypesService UnitTypesService { get; }
        public IGameResourcesService GameResourcesService { get; }

        public RewardsController(
            [NotNull] IRewardsService rewardsService,
            [NotNull] IUnitTypesService unitTypesService,
            [NotNull] IGameResourcesService gameResourcesService)
        {
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            GameResourcesService = gameResourcesService ?? throw new ArgumentNullException(nameof(gameResourcesService));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentNotAcceptedRewards()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            var rewards = await RewardsService.GetNotAcceptedPlayerRewards(playerId);
            var rewardResources = await Task.WhenAll(rewards.Select(async reward =>
            {
                var resourcesAmounts = new List<ResourceAmount[]>();
                if (reward.RewardType == RewardType.UnitsToBuy || reward.RewardType == RewardType.UnitsToUpgrade)
                {
                    var prices = await UnitTypesService.GetPrices(reward.UnitTypes);
                    var amounts = await GameResourcesService.GetResourcesAmountsFromPrices(prices);
                    resourcesAmounts.AddRange(amounts);
                }

                return new AcceptingRewardResource(reward, resourcesAmounts, GameResourcesService.GoldResourceId);
            }));
            return Ok(rewardResources);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AcceptReward(Guid id, [FromBody] AcceptRewardRequestBody body)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            if (body.Accepted)
            {
                var acceptResult = await RewardsService.AcceptRewardAsync(id, playerId, body.Amounts, body.AffectedSlots);
                return Ok(new AcceptedRewardResource(acceptResult));
            }

            var rejectResult = await RewardsService.RejectRewardAsync(id, playerId);
            return Ok(new AcceptedRewardResource(rejectResult));
        }
    }
}