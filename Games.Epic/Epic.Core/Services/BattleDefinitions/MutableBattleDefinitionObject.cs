using System;
using System.Collections.Generic;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.BattleDefinitions;

namespace Epic.Core.Services.BattleDefinitions
{
    public class MutableBattleDefinitionObject : IBattleDefinitionObject
    {
        public Guid Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsFinished { get; set; }
        public Guid ContainerId { get; set; }
        public int ExpireAtDay { get; set; }
        public IUnitsContainerObject UnitsContainerObject { get; set; }
        public IReadOnlyCollection<IGlobalUnitObject> Units { get; set; }
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
                ContainerId = entity.ContainerId,
                IsFinished = entity.Finished,
                ExpireAtDay = entity.ExpireAtDay,
            };
        }

        public IBattleDefinitionEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}