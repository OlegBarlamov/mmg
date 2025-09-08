using System;
using Epic.Core.Logic;
using Epic.Core.Services.Battles;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Data.GameResources;
using Epic.Logic.Generator;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Logic.Battle
{
    [UsedImplicitly]
    public class BattleLogicFactory : IBattleLogicFactory
    {
        [NotNull] public IBattleUnitsService BattleUnitsService { get; }
        [NotNull] public IGlobalUnitsService GlobalUnitsService { get; }
        [NotNull] public IBattlesService BattlesService { get; }
        [NotNull] public IRewardsService RewardsService { get; }
        public IDaysProcessor DaysProcessor { get; }
        public IPlayersService PlayersService { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IRandomService RandomProvider { get; }
        public IHeroesService HeroesService { get; }
        public IGameResourcesRepository ResourcesRepository { get; }

        public BattleLogicFactory(
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService,
            [NotNull] IDaysProcessor daysProcessor,
            [NotNull] IPlayersService playersService,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IRandomService randomProvider,
            [NotNull] IHeroesService heroesService,
            [NotNull] IGameResourcesRepository resourcesRepository)
        {
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            DaysProcessor = daysProcessor ?? throw new ArgumentNullException(nameof(daysProcessor));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            RandomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
            ResourcesRepository = resourcesRepository ?? throw new ArgumentNullException(nameof(resourcesRepository));
        }
        
        public IBattleLogic Create(MutableBattleObject battleObject, IBattleMessageBroadcaster broadcaster)
        {
            return new BattleLogic(battleObject,
                BattleUnitsService,
                GlobalUnitsService,
                BattlesService,
                RewardsService,
                broadcaster,
                DaysProcessor,
                PlayersService,
                LoggerFactory.CreateLogger<BattleLogic>(),
                RandomProvider,
                HeroesService,
                ResourcesRepository);
        }
    }
}