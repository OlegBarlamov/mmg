using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Data.UnitTypes;

namespace Epic.Core.Services.UnitTypes
{
    public interface IUnitTypeObject : IGameObject<IUnitTypeEntity>, IUnitProps
    {
        Guid Id { get; }
        string Name { get; }
        string BattleImgUrl { get; }
        string DashboardImgUrl { get; }
        int Value { get; }
        IReadOnlyDictionary<string, int> ResourcesDistribution { get; } 
    }
}