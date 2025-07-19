using System;
using System.Collections.Generic;
using Epic.Core.Objects;

namespace Epic.Core
{
    public interface IBattleDefinitionObject
    {
        Guid Id { get; }

        int Width { get; }
        int Height { get; }

        IReadOnlyCollection<IPlayerUnitObject> Units { get; }
    }
}