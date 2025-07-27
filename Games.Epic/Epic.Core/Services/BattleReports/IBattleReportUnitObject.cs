using System;
using Epic.Core.Objects;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitTypes;
using Epic.Data.BattleUnits;
using Epic.Data.PlayerUnits;

namespace Epic.Core.Services.BattleReports
{
    public interface IBattleReportUnitObject : IGameObject<IBattleUnitEntity>
    {
        Guid Id { get; }
        
        int PlayerIndex { get; }
        int InitialCount { get; }
        int ActualCount { get; }
        int Column { get; }
        int Row { get; }
        
        IUnitTypeObject UnitType { get; }
    }

    internal class MutableBattleReportUnitObject : IBattleReportUnitObject
    {
        public Guid Id { get; set; }

        public int PlayerIndex { get; set; }

        public int InitialCount { get; set; }
        public int ActualCount { get; set; }

        public int Column { get; set; }

        public int Row { get; set; }

        public IUnitTypeObject UnitType { get; set; }

        private MutableBattleReportUnitObject(Guid id)
        {
            Id = id;
        }

        public static MutableBattleReportUnitObject FromEntity(IBattleUnitEntity entity, IUnitTypeObject unitType)
        {
            return new MutableBattleReportUnitObject(entity.Id)
            {
                PlayerIndex = entity.PlayerIndex,
                InitialCount = entity.InitialCount,
                Column = entity.Column,
                Row = entity.Row,
                UnitType = unitType,
                ActualCount = entity.CurrentCount,
            };
        }
        
        public IBattleUnitEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}