using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Services.Units;
using Epic.Data.BattleDefinitions;

namespace Epic.Core.Services.BattleDefinitions
{
    public interface IBattleDefinitionObject : IGameObject<IBattleDefinitionEntity>
    {
        Guid Id { get; }

        int Width { get; }
        int Height { get; }
        
        bool IsFinished { get; }

        IReadOnlyCollection<IPlayerUnitObject> Units { get; }
    }
}