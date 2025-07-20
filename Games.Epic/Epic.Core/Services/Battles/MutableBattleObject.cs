using System;
using System.Collections.Generic;
using Epic.Data.Battles;

namespace Epic.Core.Services.Battles
{
    public class MutableBattleObject : IBattleObject
    {
        public Guid Id { get; set; }
        public Guid BattleDefinitionId { get; set; }
        public int TurnNumber { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsActive { get; set; }
        public int TurnPlayerIndex { get; set; }
        public int LastTurnUnitIndex { get; set; }

        public List<Guid> PlayerIds { get; set; }
        public List<MutableBattleUnitObject> Units { get; set; }

        IReadOnlyList<Guid> IBattleObject.PlayersIds => PlayerIds;
        IReadOnlyCollection<IBattleUnitObject> IBattleObject.Units => Units;

        private MutableBattleObject()
        {
            
        }
        
        public Guid? GetPlayerId(InBattlePlayerNumber playerNumber)
        {
            int index = (int)playerNumber - 1;
            if (index < 0 || index >= PlayerIds.Count)
                return null;
            
            return PlayerIds[index];
        }
        
        internal static MutableBattleObject FromEntity(IBattleEntity entity)
        {
            return new MutableBattleObject
            {
                Id = entity.Id,
                BattleDefinitionId = entity.BattleDefinitionId,
                TurnNumber = entity.TurnNumber,
                Width = entity.Width,
                Height = entity.Height,
                IsActive = entity.IsActive,
                Units = null,
                TurnPlayerIndex = 0,
                LastTurnUnitIndex = entity.LastTurnUnitIndex,
            };
        }
        
        internal static IBattleEntity ToEntity(IBattleObject battleObject)
        {
            return new MutableBattleEntity
            {
                Id = battleObject.Id,
                BattleDefinitionId = battleObject.BattleDefinitionId,
                TurnNumber = battleObject.TurnNumber,
                Width = battleObject.Width,
                Height = battleObject.Height,
                IsActive = battleObject.IsActive,
                LastTurnUnitIndex = battleObject.LastTurnUnitIndex,
            };
        }

        public IBattleEntity ToEntity()
        {
            return ToEntity(this);
        }
    }
}