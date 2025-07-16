using System;
using Epic.Data.UnitTypes;

namespace Epic.Core
{
    public interface IUnitTypeObject : IUnitProps
    {
        Guid Id { get; }
        string Name { get; }
        string BattleImgUrl { get; }
        string DashboardImgUrl { get; }
    }
}