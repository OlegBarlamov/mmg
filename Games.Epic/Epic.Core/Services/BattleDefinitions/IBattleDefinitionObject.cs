using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.BattleDefinitions;

namespace Epic.Core.Services.BattleDefinitions
{
    public interface IBattleDefinitionObject : IGameObject<IBattleDefinitionEntity>
    {
        Guid Id { get; }

        int Width { get; }
        int Height { get; }
        
        bool IsFinished { get; }
        
        Guid ContainerId { get; }
        
        IUnitsContainerObject UnitsContainerObject { get; }
        
        IReadOnlyCollection<IRewardObject> Rewards { get; }

        IReadOnlyCollection<IPlayerUnitObject> Units { get; }
    }
}