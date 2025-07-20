using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Data.BattleDefinitions;

namespace Epic.Core.Services.BattleDefinitions
{
    public class MutableBattleDefinitionObject : IBattleDefinitionObject
    {
        public Guid Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsFinished { get; set; }
        public IReadOnlyCollection<Guid> UnitsIds { get; set; }
        public IReadOnlyCollection<IPlayerUnitObject> Units { get; set; }
        public IReadOnlyCollection<Guid> RewardsIds { get; set; }
        public IReadOnlyCollection<IRewardObject> Rewards { get; set; }

        private MutableBattleDefinitionObject()
        {
        }

        internal static MutableBattleDefinitionObject FromEntity(IBattleDefinitionEntity entity)
        {
            return new MutableBattleDefinitionObject
            {
                Id = entity.Id,
                Width = entity.Width,
                Height = entity.Height,
                UnitsIds = entity.UnitsIds,
                IsFinished = entity.Finished,
            };
        }

        public IBattleDefinitionEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}