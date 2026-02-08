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
using static Epic.Logic.Battle.BattleUnitBuffExtensions;
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
        private IBuffsLogic BuffsLogic { get; }

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
            
            BuffsLogic = new BuffsLogic(
                BuffsService,
                BuffTypesService,
                RandomProvider,
                ClientConnectedHandler,
                new EffectsLogic(GlobalUnitsService, BattleUnitsService, ClientConnectedHandler));
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
                    await InitializeUnitsCurrentHealth();
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
                    await BuffsLogic.ProcessActiveUnitBuffs(
                        (MutableBattleUnitObject)BattleUnitsCarousel.ActiveUnit,
                        BattleObject.TurnNumber,
                        BattleUnitsCarousel.ActiveUnit.PlayerIndex.ToInBattlePlayerNumber());
                    
                    // Check if unit can act (not paralyzed) - include this info in NEXT_TURN message
                    var unitCanAct = BattleUnitsCarousel.ActiveUnit.CanAct();
                    
                    await ClientConnectedHandler.BroadcastMessageToClientAndSaveAsync(
                        new NextTurnCommandFromServer(
                            BattleObject.TurnNumber,
                            BattleUnitsCarousel.ActiveUnit.PlayerIndex.ToInBattlePlayerNumber(),
                            BattleUnitsCarousel.ActiveUnit.Id,
                            BattleObject.RoundNumber,
                            unitCanAct));

                    cancellationToken.ThrowIfCancellationRequested();

                    // Check if unit is paralyzed - auto-pass if so
                    if (!unitCanAct)
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

        private async Task InitializeUnitsCurrentHealth()
        {
            foreach (var unit in BattleObject.Units)
            {
                unit.CurrentHealth = unit.GetEffectiveMaxHealth();
            }
            await BattleUnitsService.UpdateUnits(BattleObject.Units);
        }

        private bool IsHumanControlled(IBattleUnitObject unit, out bool runClaimed)
        {
            var playerInfo = BattleObject.FindPlayerInfo(unit.PlayerIndex.ToInBattlePlayerNumber());
            runClaimed = playerInfo is { RunClaimed: true };
            return playerInfo != null;
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
                BuffsLogic);
            
            await handler.Validate(context, command);
            
            await connection.SendMessageAsync(new CommandApproved(command));
            
            var result = await handler.Execute(context, command);
            
            if (result.TurnFinished)
                TurnAwaiter.TurnProcessed(command.Command);
        }
    }
}