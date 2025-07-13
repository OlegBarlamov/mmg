using System;
using Epic.Data.UnitTypes;

namespace Epic.Core
{
    public interface IUnitTypeObject : IUnitProps
    {
        Guid Id { get; }
        string BattleImgUrl { get; }
        string DashboardImgUrl { get; }
    }
}