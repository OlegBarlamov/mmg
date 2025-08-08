using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.BattleDefinitions;

namespace Epic.Core.Services.BattleDefinitions
{
    public interface IBattleDefinitionObject : IGameObject<IBattleDefinitionEntity>, ISize
    {
        Guid Id { get; }
        
        bool IsFinished { get; }
        
        Guid ContainerId { get; }
        
        int ExpireAtDay { get; }
        
        IUnitsContainerObject UnitsContainerObject { get; }
        
        IReadOnlyCollection<IRewardObject> Rewards { get; }

        IReadOnlyCollection<IGlobalUnitObject> Units { get; }
    }
}