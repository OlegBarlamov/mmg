using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Logic.Generator
{
    public interface IDaysProcessor
    {
        Task ProcessNewDay(IReadOnlyList<Guid> playerIds);
    }
    
    [UsedImplicitly]
    public class DaysProcessor : IDaysProcessor
    {
        public IPlayersService PlayersService { get; }
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public ILogger<DaysProcessor> Logger { get; }
        public IBattlesGenerator BattlesGenerator { get; }

        public DaysProcessor(
            [NotNull] IPlayersService playersService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] ILogger<DaysProcessor> logger,
            [NotNull] IBattlesGenerator battlesGenerator)
        {
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            BattlesGenerator = battlesGenerator ?? throw new ArgumentNullException(nameof(battlesGenerator));
        }

        public async Task ProcessNewDay(IReadOnlyList<Guid> playerIds)
        {
            try
            {
                await PlayersService.DayIncrement(playerIds);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Unexpected error during day increment: {e.Message}");
            }

            Task.Run(async () => { await Task.WhenAll(playerIds.Select(ProcessNewDayForPlayer)); });
        }

        private async Task ProcessNewDayForPlayer(Guid playerId)
        {
            try
            {
                await ProcessNewDayForPlayerInternal(playerId);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Unexpected error during day processing for player {playerId}: {e.Message}");
            }
        }

        private async Task ProcessNewDayForPlayerInternal(Guid playerId)
        {
            var player = await PlayersService.GetById(playerId);
            if (player.PlayerType == PlayerObjectType.Computer)
                return;
            
            var globalDay = player.Day;
            var stageUnlockedDays = player.StageUnlockedDays ?? new[] { 1 };
            
            await PlayersService.SetGenerationInProgress(playerId, true);
            try
            {
                // Generate battles for all unlocked stages
                for (int stage = 0; stage <= player.Stage; stage++)
                {
                    // Calculate effective day for this stage (frozen until unlocked)
                    var stageUnlockedDay = stage < stageUnlockedDays.Length ? stageUnlockedDays[stage] : 1;
                    var effectiveDay = Math.Max(1, globalDay - stageUnlockedDay + 1);
                    
                    var battlesCount = await BattleDefinitionsService.GetBattlesCountForPlayer(player);
                    
                    Logger.LogInformation($"Generating battles for player {playerId}, stage {stage}, effectiveDay {effectiveDay}, globalDay {globalDay}");
                    await BattlesGenerator.Generate(player.Id, effectiveDay, globalDay, battlesCount, stage);
                }
            }
            finally
            {
                await PlayersService.SetGenerationInProgress(playerId, false);
            }
        }
    }
}