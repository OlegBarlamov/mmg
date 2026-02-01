using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ClientMessages;
using Epic.Core.Logic;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.Connection;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Data.GameResources;
using Epic.Logic.Battle.Commands;
using Epic.Logic.Generator;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Logic.Battle
{
    public class BattleLogic : IBattleLogic
    {
        private MutableBattleObject BattleObject { get; }
        private IBattleUnitsService BattleUnitsService { get; }
        private IGlobalUnitsService GlobalUnitsService { get; }
        private IBattlesService BattlesService { get; }
        private IRewardsService RewardsService { get; }
        private IBattleMessageBroadcaster Broadcaster { get; }
        private IDaysProcessor DaysProcessor { get; }
        private IPlayersService PlayersService { get; }
        private ILogger<BattleLogic> Logger { get; }
        private IRandomService RandomProvider { get; }
        private IHeroesService HeroesService { get; }
        public IGameResourcesRepository ResourcesRepository { get; }
        private IBuffsService BuffsService { get; }
        private IBuffTypesService BuffTypesService { get; }

        private BattleResultLogic BattleResultLogic { get; }
        private BattleUnitsCarousel BattleUnitsCarousel { get; }
        private TurnAwaiter TurnAwaiter { get; }
        private ClientConnectedHandler ClientConnectedHandler { get; }
        private BattleAI AI { get; }

        private Dictionary<Type, ICommandsHandler> CommandsHandlers { get; } =
            new Dictionary<Type, ICommandsHandler>
            {
                { typeof(UnitPassClientBattleMessage), new UnitPassesHandler() },
                { typeof(UnitWaitClientBattleMessage), new UnitWaitsHandler() },
                { typeof(UnitMoveClientBattleMessage), new UnitMovesHandler() },
                { typeof(UnitAttackClientBattleMessage), new UnitAttacksHandler() },
                { typeof(PlayerRansomClientBattleMessage), new PlayerRansomHandler() },
                { typeof(PlayerRunClientBattleMessage), new PlayerRunHandler() },
            };
        
        private bool _isDisposed;

        public BattleLogic(
            [NotNull] MutableBattleObject battleObject,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService,
            [NotNull] IBattleMessageBroadcaster broadcaster,
            [NotNull] IDaysProcessor daysProcessor,
            [NotNull] IPlayersService playersService,
            [NotNull] ILogger<BattleLogic> logger,
            [NotNull] IRandomService randomProvider,
            [NotNull] IHeroesService heroesService,
            [NotNull] IGameResourcesRepository resourcesRepository,
            [NotNull] IBuffsService buffsService,
            [NotNull] IBuffTypesService buffTypesService)
        {
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            Broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            DaysProcessor = daysProcessor ?? throw new ArgumentNullException(nameof(daysProcessor));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RandomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
            ResourcesRepository = resourcesRepository ?? throw new ArgumentNullException(nameof(resourcesRepository));
            BuffsService = buffsService ?? throw new ArgumentNullException(nameof(buffsService));
            BuffTypesService = buffTypesService ?? throw new ArgumentNullException(nameof(buffTypesService));


            ClientConnectedHandler = new ClientConnectedHandler(Broadcaster);
            CommandsHandlers.Add(typeof(ClientConnectedBattleMessage), ClientConnectedHandler);
            
            BattleUnitsCarousel = new BattleUnitsCarousel(
                BattleObject,
                BattleUnitsService,
                BattlesService);
            BattleResultLogic = new BattleResultLogic(
                RewardsService,
                HeroesService,
                BattlesService,
                DaysProcessor,
                PlayersService);
            TurnAwaiter = new TurnAwaiter();
            
            AI = new BattleAI(BattleObject, this);
        }

        public void Dispose()
        {
            _isDisposed = true;
            BattleUnitsCarousel.Dispose();
            TurnAwaiter.Dispose();
            ClientConnectedHandler.Dispose();
        }

        public async Task<BattleResult> Run(CancellationToken cancellationToken)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BattleLogic));

            try
            {
                if (BattleObject.TurnNumber < 0 && !BattleResultLogic.UpdateIsBattleFinished(BattleObject))
                {
                    // Initialize the battle
                    BattleObject.TurnNumber = 0;
                    await BattlesService.UpdateBattle(BattleObject);
                    await ClientConnectedHandler.WaitForFirstClientConnected();
                    
                    cancellationToken.ThrowIfCancellationRequested();
                }

                while (!BattleResultLogic.UpdateIsBattleFinished(BattleObject))
                {
                    BattleUnitsCarousel.ProcessNextTurn();
                    if (BattleUnitsCarousel.ActiveUnit.PlayerIndex != BattleObject.TurnPlayerIndex)
                    {
                        // should be called only once when the first active unit is found
                        BattleObject.TurnPlayerIndex = BattleUnitsCarousel.ActiveUnit.PlayerIndex;
                        await BattlesService.UpdateBattle(BattleObject);
                    }
                    
                    // Process buff expiration before the unit takes its turn
                    await ProcessActiveUnitBuffs(BattleUnitsCarousel.ActiveUnit);
                    
                    await ClientConnectedHandler.BroadcastMessageToClientAndSaveAsync(
                        new NextTurnCommandFromServer(
                            BattleObject.TurnNumber,
                            BattleUnitsCarousel.ActiveUnit.PlayerIndex.ToInBattlePlayerNumber(),
                            BattleUnitsCarousel.ActiveUnit.Id,
                            BattleObject.RoundNumber));

                    cancellationToken.ThrowIfCancellationRequested();

                    // Check if unit is paralyzed - auto-pass if so
                    if (IsUnitParalyzed(BattleUnitsCarousel.ActiveUnit))
                    {
                        Logger.LogInformation(
                            $"Unit is paralyzed, auto-pass: {BattleUnitsCarousel.ActiveUnit.Id} - Turn {BattleObject.TurnNumber}");
                        TurnAwaiter.WaitForAiTurn(BattleUnitsCarousel.ActiveUnit.PlayerIndex, BattleObject.TurnNumber);
                        await AI.ProcessAutoSkip(BattleUnitsCarousel.ActiveUnit);
                    }
                    else if (IsHumanControlled(BattleUnitsCarousel.ActiveUnit, out var runClaimed))
                    {
                        if (runClaimed)
                        {
                            TurnAwaiter.WaitForAiTurn(BattleUnitsCarousel.ActiveUnit.PlayerIndex, BattleObject.TurnNumber);
                            await AI.ProcessAutoSkip(BattleUnitsCarousel.ActiveUnit);
                        }
                        else
                        {
                            Logger.LogInformation(
                                $"Await player turn: {BattleUnitsCarousel.ActiveUnit.PlayerIndex.ToInBattlePlayerNumber()} - {BattleObject.TurnNumber}");
                            await TurnAwaiter.WaitForClientTurn(BattleUnitsCarousel.ActiveUnit.PlayerIndex, BattleObject.TurnNumber);
                        }
                    }
                    else
                    {
                        Logger.LogInformation(
                            $"Await AI turn: {BattleUnitsCarousel.ActiveUnit.PlayerIndex.ToInBattlePlayerNumber()} - {BattleObject.TurnNumber}");
                        TurnAwaiter.WaitForAiTurn(BattleUnitsCarousel.ActiveUnit.PlayerIndex, BattleObject.TurnNumber);
                        await AI.ProcessAction(BattleUnitsCarousel.ActiveUnit);
                    }
                    
                    Logger.LogInformation($"Turn finished: {BattleObject.TurnNumber}");

                    if (BattleUnitsCarousel.IsNextRound())
                    {
                        await BattleUnitsCarousel.OnNextRound();
                        await UpdateRunPlayers();
                    }

                    await BattleUnitsCarousel.OnNextTurn();

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                Logger.LogError($"Unexpected error in battle logic cycle: {e}");
            }

            if (BattleResultLogic.IsFinished)
            {
                var battleFinishCommand = await BattleResultLogic.OnBattleFinished(BattleObject);
                await ClientConnectedHandler.BroadcastMessageToClientAndSaveAsync(battleFinishCommand);
            }

            return BattleResultLogic.BattleResult;
        }

        private bool IsHumanControlled(IBattleUnitObject unit, out bool runClaimed)
        {
            var playerInfo = BattleObject.FindPlayerInfo(unit.PlayerIndex.ToInBattlePlayerNumber());
            runClaimed = playerInfo is { RunClaimed: true };
            return playerInfo != null;
        }

        /// <summary>
        /// Checks if the unit has any buff with Paralyzed effect.
        /// </summary>
        private static bool IsUnitParalyzed(IBattleUnitObject unit)
        {
            if (unit?.Buffs == null || unit.Buffs.Count == 0)
                return false;

            return unit.Buffs.Any(buff => buff.BuffType?.Paralyzed == true);
        }

        private async Task UpdateRunPlayers()
        {
            var runPlayers = BattleObject.PlayerInfos.Where(x => x.RunClaimed);
            await Task.WhenAll(runPlayers.Select(x =>
            {
                x.RunInfo.RunRoundsRemaining--;
                return BattlesService.UpdateInBattlePlayerInfo(x);
            }));
        } 

        public async Task OnClientMessage(IBattleClientConnection connection, IClientBattleMessage command)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BattleLogic));
            
            if (!CommandsHandlers.TryGetValue(command.GetType(), out var handler)) 
                throw new ClientCommandRejected("Unknown client command");

            var context = new CommandExecutionContext(
                BattleObject, 
                TurnAwaiter.ExpectedTurn, 
                ClientConnectedHandler, 
                BattleUnitsService, 
                BattleUnitsCarousel,
                GlobalUnitsService,
                RandomProvider,
                connection,
                BattlesService,
                ResourcesRepository,
                BuffsService,
                BuffTypesService);
            
            await handler.Validate(context, command);
            
            await connection.SendMessageAsync(new CommandApproved(command));
            
            var result = await handler.Execute(context, command);
            
            if (result.TurnFinished)
                TurnAwaiter.TurnProcessed(command.Command);
        }

        /// <summary>
        /// Processes buff expiration for the active unit before its turn.
        /// Decrements duration for non-permanent buffs and removes expired ones.
        /// </summary>
        private async Task ProcessActiveUnitBuffs(IBattleUnitObject activeUnit)
        {
            if (activeUnit?.Buffs == null || activeUnit.Buffs.Count == 0)
                return;

            var buffsToUpdate = new List<IBuffObject>();
            var buffsToRemove = new List<IBuffObject>();
            var remainingBuffs = new List<IBuffObject>();

            foreach (var buff in activeUnit.Buffs)
            {
                // Permanent buffs (DurationRemaining = 0 and Permanent = true) never expire
                if (buff.BuffType?.Permanent == true)
                {
                    remainingBuffs.Add(buff);
                    continue;
                }

                // Non-permanent buffs: decrement duration first
                if (buff is MutableBuffObject mutableBuff)
                {
                    mutableBuff.DurationRemaining--;
                    
                    // Remove buff only when duration goes below 0
                    if (mutableBuff.DurationRemaining < 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                    else
                    {
                        buffsToUpdate.Add(buff);
                        remainingBuffs.Add(buff);
                    }
                }
            }

            // Update buffs with decremented duration
            if (buffsToUpdate.Count > 0)
            {
                await BuffsService.UpdateBatch(buffsToUpdate.ToArray());
            }

            // Remove expired buffs and notify clients
            foreach (var expiredBuff in buffsToRemove)
            {
                await BuffsService.DeleteById(expiredBuff.Id);
                
                var buffExpiredMessage = new UnitLosesBuffCommandFromServer(
                    BattleObject.TurnNumber,
                    activeUnit.PlayerIndex.ToInBattlePlayerNumber(),
                    activeUnit.Id.ToString())
                {
                    BuffId = expiredBuff.Id.ToString(),
                    BuffName = expiredBuff.BuffType?.Name ?? "Unknown"
                };
                await ClientConnectedHandler.BroadcastMessageToClientAndSaveAsync(buffExpiredMessage);
            }

            // Update the unit's in-memory buffs list
            if (buffsToRemove.Count > 0 && activeUnit is MutableBattleUnitObject mutableUnit)
            {
                mutableUnit.Buffs = remainingBuffs;
            }
            
            // Process healing from active buffs
            await ProcessBuffHealing(activeUnit, remainingBuffs);
        }

        /// <summary>
        /// Processes healing effects from active buffs at the start of a unit's turn.
        /// </summary>
        private async Task ProcessBuffHealing(IBattleUnitObject activeUnit, List<IBuffObject> activeBuffs)
        {
            if (activeBuffs == null || activeBuffs.Count == 0)
                return;

            // Calculate total healing from all buffs
            var totalFlatHeal = 0;
            var totalPercentageHeal = 0;
            var canResurrect = false;

            foreach (var buff in activeBuffs)
            {
                if (buff.BuffType == null)
                    continue;

                totalFlatHeal += buff.BuffType.Heals;
                totalPercentageHeal += buff.BuffType.HealsPercentage;
                if (buff.BuffType.HealCanResurrect)
                    canResurrect = true;
            }

            if (totalFlatHeal <= 0 && totalPercentageHeal <= 0)
                return;

            var mutableUnit = activeUnit as MutableBattleUnitObject;
            if (mutableUnit == null)
                return;

            var unitHealth = mutableUnit.GlobalUnit.UnitType.Health;
            var initialCount = mutableUnit.InitialCount;
            var currentCount = mutableUnit.CurrentCount;
            var currentHealth = mutableUnit.CurrentHealth;

            // Calculate HP to heal
            var percentageHeal = unitHealth * totalPercentageHeal / 100;
            var hpToHeal = totalFlatHeal + percentageHeal;

            if (hpToHeal <= 0)
                return;

            // Calculate new health values
            var totalCurrentHp = (currentCount - 1) * unitHealth + currentHealth;
            var totalNewHp = totalCurrentHp + hpToHeal;

            // Cap at initial army size if resurrection is not allowed
            var maxTotalHp = canResurrect
                ? initialCount * unitHealth
                : currentCount * unitHealth;

            if (totalNewHp > maxTotalHp)
                totalNewHp = maxTotalHp;

            // Calculate new count and health
            var newCount = (totalNewHp + unitHealth - 1) / unitHealth; // Ceiling division
            var newHealth = totalNewHp - (newCount - 1) * unitHealth;

            // Ensure we don't exceed initial count
            if (newCount > initialCount)
            {
                newCount = initialCount;
                newHealth = unitHealth;
            }

            var actualHealedAmount = totalNewHp - totalCurrentHp;
            var resurrectedCount = newCount - currentCount;

            if (actualHealedAmount <= 0)
                return;

            // Update the unit's state
            mutableUnit.CurrentCount = newCount;
            mutableUnit.CurrentHealth = newHealth;
            mutableUnit.GlobalUnit.Count = newCount;

            await GlobalUnitsService.UpdateUnits(new[] { mutableUnit.GlobalUnit });
            await BattleUnitsService.UpdateUnits(new[] { mutableUnit });

            // Broadcast healing message
            var healMessage = new UnitHealsCommandFromServer(
                BattleObject.TurnNumber,
                activeUnit.PlayerIndex.ToInBattlePlayerNumber(),
                activeUnit.Id.ToString())
            {
                HealedAmount = actualHealedAmount,
                ResurrectedCount = resurrectedCount,
                NewCount = newCount,
                NewHealth = newHealth
            };
            await ClientConnectedHandler.BroadcastMessageToClientAndSaveAsync(healMessage);
        }
    }
}