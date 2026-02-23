using Epic.Core.Logic;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.MagicTypes;
using Epic.Core.Services.Players;
using Epic.Core.Services.Units;
using Epic.Data.GameResources;
using FrameworkSDK.Common;

namespace Epic.Logic.Battle.Commands
{
    internal class CommandExecutionContext
    {
        public MutableBattleObject BattleObject { get; }
        public TurnInfo ExpectedTurn { get; }
        public IBattleUnitsService BattleUnitsService { get; }
        public BattleUnitsCarousel UnitsCarousel { get; }
        public IBattleMessageBroadcaster MessageBroadcaster { get; }
        public IGlobalUnitsService GlobalUnitsService { get; }
        public IRandomService RandomProvider { get; }
        public IBattleClientConnection Connection { get; }
        public IBattlesService BattlesService { get; }
        public IGameResourcesRepository GameResourcesRepository { get; }
        public IBuffsLogic BuffsLogic { get; }
        public IHeroesService HeroesService { get; }
        public IPlayersService PlayersService { get; }
        public IMagicTypesService MagicTypesService { get; }

        public CommandExecutionContext(
            MutableBattleObject battleObject,
            TurnInfo expectedTurn,
            IBattleMessageBroadcaster battleMessageBroadcaster,
            IBattleUnitsService battleUnitsService,
            BattleUnitsCarousel unitsCarousel,
            IGlobalUnitsService globalUnitsService,
            IRandomService randomProvider,
            IBattleClientConnection connection,
            IBattlesService battlesService,
            IGameResourcesRepository gameResourcesRepository,
            IBuffsLogic buffsLogic,
            IHeroesService heroesService,
            IPlayersService playersService,
            IMagicTypesService magicTypesService)
        {
            BattleObject = battleObject;
            ExpectedTurn = expectedTurn;
            MessageBroadcaster = battleMessageBroadcaster;
            BattleUnitsService = battleUnitsService;
            UnitsCarousel = unitsCarousel;
            Connection = connection;
            RandomProvider = randomProvider;
            GlobalUnitsService = globalUnitsService;
            BattlesService = battlesService;
            GameResourcesRepository = gameResourcesRepository;
            BuffsLogic = buffsLogic;
            HeroesService = heroesService;
            PlayersService = playersService;
            MagicTypesService = magicTypesService;
        }
    }
}