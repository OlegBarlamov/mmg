using System;
using System.Linq;
using Epic.Core.Services.BattleReports;

namespace Epic.Server.Resources
{
    public class BattleReportResource
    {
        public bool IsWinner { get; set; }
        
        public BattleReportUnitResource[] PlayerUnits { get; }
        
        public BattleReportUnitResource[] EnemyUnits { get; }
        
        public BattleReportResource(IBattleReportObject battleReportObject, Guid playerId)
        {
            var playerIndex = battleReportObject.GetPlayerIndex(playerId);
            IsWinner = battleReportObject.PlayerWinnerId == playerId;
            PlayerUnits = battleReportObject.Units.Where(x => x.PlayerIndex == playerIndex)
                .Select(x => new BattleReportUnitResource(x)).ToArray();
            EnemyUnits = battleReportObject.Units.Where(x => x.PlayerIndex != playerIndex)
                .Select(x => new BattleReportUnitResource(x)).ToArray();
        }
    }

    public class BattleReportUnitResource
    {
        public string ThumbnailUrl { get; }
        public int StartCount { get; }
        public int FinalCount { get; }
        public string Name { get; }
        
        public BattleReportUnitResource(IBattleReportUnitObject battleReportUnitObject)
        {
            ThumbnailUrl = battleReportUnitObject.UnitType.BattleImgUrl;
            StartCount = battleReportUnitObject.InitialCount;
            FinalCount = battleReportUnitObject.ActualCount;
            Name = battleReportUnitObject.UnitType.Name;
        }
    }
}