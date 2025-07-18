using System;
using Epic.Core.Objects.Battle;

namespace Epic.Core.Services.Battles
{
    public interface IBattlesCacheService
    {
        MutableBattleObject FindBattleById(Guid battleId);
        void AddIfAbsent(MutableBattleObject battleObject);
        void ReleaseBattle(Guid battleId);
    }
}