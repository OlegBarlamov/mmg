using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Services.Battles;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.Heroes;
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
        public IPlayersService PlayersService { get; }
        public IBuffTypesRegistry BuffTypesRegistry { get; }

        public RewardsController(
            [NotNull] IRewardsService rewardsService,
            [NotNull] IUnitTypesService unitTypesService,
            [NotNull] IGameResourcesService gameResourcesService,
            [NotNull] IPlayersService playersService,
            [NotNull] IBuffTypesRegistry buffTypesRegistry)
        {
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            GameResourcesService = gameResourcesService ?? throw new ArgumentNullException(nameof(gameResourcesService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            BuffTypesRegistry = buffTypesRegistry ?? throw new ArgumentNullException(nameof(buffTypesRegistry));
        }

        private async Task<Dictionary<Guid, IHeroStats>> GetPlayerHeroStats(IBattleObject battleObject)
        {
            var playerIds = battleObject.PlayerInfos.Select(x => x.PlayerId).ToList();
            var players = await PlayersService.GetByIds(playerIds.ToArray());
            return players.ToDictionary(
                p => p.Id,
                p => p.ActiveHero.GetCumulativeHeroStats()
            );
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

        [HttpPost("{rewardId}/guard")]
        public async Task<IActionResult> BeginRewardGuardBattle(Guid rewardId)
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            var battleObject = await RewardsService.BeginRewardGuardBattle(rewardId, playerId);
            var playerHeroStats = await GetPlayerHeroStats(battleObject);
            return Ok(new BattleResource(battleObject, playerHeroStats, BuffTypesRegistry));
        }
    }
}