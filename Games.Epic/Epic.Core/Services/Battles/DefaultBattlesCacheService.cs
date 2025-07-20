using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Epic.Core.Services.Battles
{
    [UsedImplicitly]
    public class DefaultBattlesCacheService : IBattlesCacheService
    {
        private readonly ConcurrentDictionary<Guid, MutableBattleObject> _battleObjects = new ConcurrentDictionary<Guid, MutableBattleObject>();
        
        public MutableBattleObject FindBattleById(Guid battleId)
        {
            return _battleObjects.GetValueOrDefault(battleId);
        }

        public void AddIfAbsent(MutableBattleObject battleObject)
        {
            _battleObjects.TryAdd(battleObject.Id, battleObject);
        }

        public void ReleaseBattle(Guid battleId)
        {
            _battleObjects.TryRemove(battleId, out _);
        }
    }
}