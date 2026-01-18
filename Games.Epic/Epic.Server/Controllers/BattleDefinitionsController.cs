using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Logic.Descriptions;
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
        public IGameResourcesService GameResourcesService { get; }
        public IGameModeProvider GameModeProvider { get; }

        public BattleDefinitionsController(
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IRewardsService rewardsService,
            [NotNull] IPlayersService playersService,
            [NotNull] IGameResourcesService gameResourcesService,
            [NotNull] IGameModeProvider gameModeProvider)
        {
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            GameResourcesService = gameResourcesService ?? throw new ArgumentNullException(nameof(gameResourcesService));
            GameModeProvider = gameModeProvider ?? throw new ArgumentNullException(nameof(gameModeProvider));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserBattleDefinitions()
        {
            if (!User.TryGetPlayerId(out var playerId))
                return BadRequest(Constants.PlayerIdIsNotSpecifiedErrorMessage);
            
            var player = await PlayersService.GetById(playerId);
            var battles = await BattleDefinitionsService.GetNotExpiredActiveBattleDefinitionsByPlayerAsync(playerId);
            var rewards = await RewardsService.GetRewardsFromBattleDefinitions(battles.Select(x => x.Id).ToArray());
            var gameMode = GameModeProvider.GetGameMode();
            var battlesResources = battles.Select(x => 
            {
                var gameStage = gameMode.Stages[Math.Min(x.Stage, gameMode.Stages.Length - 1)];
                var rewardFactor = gameStage.RewardsFactor;
                return new BattleDefinitionResource(
                    x, 
                    rewards[x.Id],
                    AdjustVisibility(DescriptionVisibility.MaskedSize, x.RewardVisibility),
                    AdjustVisibility(DescriptionVisibility.MaskedSize, x.GuardVisibility),
                    GameResourcesService.GoldResourceId,
                    player,
                    rewardFactor);
            });
            
            return Ok(battlesResources);
        }

        private DescriptionVisibility AdjustVisibility(DescriptionVisibility defaultVisibility, int factor)
        {
            return (DescriptionVisibility)((int)defaultVisibility + factor);
        }
    }
}