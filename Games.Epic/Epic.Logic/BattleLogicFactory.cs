using System;
using Epic.Core;
using Epic.Core.Logic;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleGameManager;
using Epic.Core.Services;
using JetBrains.Annotations;

namespace Epic.Logic
{
    [UsedImplicitly]
    public class BattleLogicFactory : IBattleLogicFactory
    {
        [NotNull] public IBattleUnitsService BattleUnitsService { get; }
        [NotNull] public IUserUnitsService UserUnitsService { get; }
        [NotNull] public IBattlesService BattlesService { get; }
        [NotNull] public IRewardsService RewardsService { get; }

        public BattleLogicFactory(
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IUserUnitsService userUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService)
        {
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
        }
        
        public IBattleLogic Create(MutableBattleObject battleObject, IBattleMessageBroadcaster broadcaster)
        {
            return new BattleLogic(battleObject,
                BattleUnitsService,
                UserUnitsService,
                BattlesService,
                RewardsService,
                broadcaster);
        }
    }
}