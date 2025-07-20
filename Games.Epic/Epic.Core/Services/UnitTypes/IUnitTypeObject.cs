using System;
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
    }
}