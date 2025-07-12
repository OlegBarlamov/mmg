using System;
using System.Collections.Generic;
using Epic.Core.Objects.Battle;
using JetBrains.Annotations;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultBattlesCacheService : IBattlesCacheService
    {
        private readonly Dictionary<Guid, MutableBattleObject> _battleObjects = new Dictionary<Guid, MutableBattleObject>();
        
        public MutableBattleObject FindBattleById(Guid battleId)
        {
            return _battleObjects.GetValueOrDefault(battleId);
        }

        public void AddBattle(MutableBattleObject battleObject)
        {
            if (!_battleObjects.TryAdd(battleObject.Id, battleObject))
                throw new ArgumentException($"Battle with id {battleObject.Id} already exists");
        }

        public void ReleaseBattle(Guid battleId)
        {
            _battleObjects.Remove(battleId);
        }
    }
}