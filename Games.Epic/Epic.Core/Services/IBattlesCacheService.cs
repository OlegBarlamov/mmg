using System;
using Epic.Core.Objects.Battle;

namespace Epic.Core
{
    public interface IBattlesCacheService
    {
        IBattleObject FindBattleById(Guid battleId);
        void AddBattle(IBattleObject battleObject);
        void ReleaseBattle(IBattleObject battleObject);
    }
}