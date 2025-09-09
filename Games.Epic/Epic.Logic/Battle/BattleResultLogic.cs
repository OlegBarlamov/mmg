using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.Logic;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Logic.Generator;
using JetBrains.Annotations;

namespace Epic.Logic.Battle
{
    internal class BattleResultLogic
    {
        public bool IsFinished => BattleResult.Finished;

        public BattleResult BattleResult { get; private set; } = new BattleResult();
        
        private IRewardsService RewardsService { get; }
        private IHeroesService HeroesService { get; }
        private IBattlesService BattlesService { get; }
        private IDaysProcessor DaysProcessor { get; }
        private IPlayersService PlayersService { get; }

        public BattleResultLogic(
            [NotNull] IRewardsService rewardsService,
            [NotNull] IHeroesService heroesService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IDaysProcessor daysProcessor,
            [NotNull] IPlayersService playersService)
        {
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            DaysProcessor = daysProcessor ?? throw new ArgumentNullException(nameof(daysProcessor));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
        }
        
        public bool UpdateIsBattleFinished(IBattleObject battleObject)
        {
            var noAliveUnits = true;
            InBattlePlayerNumber? winner = null;
            foreach (var battleUnitObject in battleObject.Units)
            {
                var targetPlayer = battleObject.FindPlayerInfo(battleUnitObject.PlayerIndex.ToInBattlePlayerNumber());
                if (battleUnitObject.GlobalUnit.IsAlive)
                {
                    noAliveUnits = false;
                    if (targetPlayer == null || !targetPlayer.RansomClaimed)
                    {
                        var player = (InBattlePlayerNumber)battleUnitObject.PlayerIndex;
                        if (winner == null)
                            winner = player;
                        else if (player != winner)
                        {
                            winner = null;
                            break;
                        }
                    }
                }
            }

            BattleResult = new BattleResult
            {
                Finished = noAliveUnits || winner.HasValue,
                Winner = winner
            };
            
            return BattleResult.Finished;
        }
        
        public async Task<BattleFinishedCommandFromServer> OnBattleFinished(IBattleObject battleObject)
        {
            if (BattleResult.Winner != null)
                await OnPlayerWon(battleObject, BattleResult.Winner.Value);
            
            // var defeatedPlayers = Players.Where(x => x.Id != winnerPlayerId).ToArray();
            // TODO kill the heroes

            if (battleObject.ProgressDays)
                await DaysProcessor.ProcessNewDay(battleObject.PlayerInfos
                    .Select(x => x.PlayerId)
                    .ToArray());

            var reportEntity = await BattlesService.FinishBattle(battleObject, BattleResult);
            return new BattleFinishedCommandFromServer(battleObject.TurnNumber)
            {
                Winner = BattleResult.Winner?.ToString() ?? string.Empty,
                ReportId = reportEntity.Id.ToString(),
            };
        }

        private async Task OnPlayerWon(IBattleObject battleObject, InBattlePlayerNumber inBattlePlayerNumber)
        {
            var winnerId = battleObject.FindPlayerId(inBattlePlayerNumber);
            if (winnerId.HasValue)
            {
                var winnerPlayerId = winnerId.Value;
                await GiveExperience(winnerPlayerId, inBattlePlayerNumber, battleObject);
                var rewards = await RewardsService.GetRewardsFromBattleDefinition(battleObject.BattleDefinitionId);
                var rewardsIds = rewards.Select(x => x.Id).ToArray();
                await RewardsService.GiveRewardsToPlayerAsync(rewardsIds, winnerPlayerId);
            }
        }

        private async Task GiveExperience(Guid playerId, InBattlePlayerNumber winner, IBattleObject battleObject)
        {
            var unitsContributeToExperience = battleObject.Units
                .Where(x => (InBattlePlayerNumber)x.PlayerIndex != winner)
                .Where(x => !x.GlobalUnit.IsAlive)
                .ToArray();

            var experienceToGain = unitsContributeToExperience.Sum(x => x.InitialCount * x.GlobalUnit.UnitType.Value);
            if (experienceToGain > 0)
            {
                var playerObject = await PlayersService.GetById(playerId);
                await HeroesService.GiveExperience(playerObject.ActiveHero.Id, experienceToGain);
            }
        }
    }
}