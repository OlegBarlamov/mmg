using System;

namespace Epic.Data.BattleReports
{
    public interface IBattleReportEntity : IBattleReportFields
    {
        Guid Id { get; }
    }

    public interface IBattleReportFields
    {
        Guid BattleId { get; }
        int? WinnerIndex { get; }
        Guid? PlayerWinnerId { get; }
        Guid[] PlayerIds { get; }
    }

    public class MutableBattleReportFields : IBattleReportFields
    {
        public Guid BattleId { get; set; }
        public int? WinnerIndex { get; set; }
        public Guid? PlayerWinnerId { get; set; }
        public Guid[] PlayerIds { get; set; }
        
        public MutableBattleReportFields(Guid battleId, int? winnerIndex, Guid[] playerIds, Guid? playerWinnerId)
        {
            BattleId = battleId;
            WinnerIndex = winnerIndex;
            PlayerIds = playerIds;
            PlayerWinnerId = playerWinnerId;
        }
    }

    internal class MutableBattleReportEntity : MutableBattleReportFields, IBattleReportEntity
    {
        public Guid Id { get; }
        
        internal MutableBattleReportEntity(Guid id, IBattleReportFields fields)
            : base(fields.BattleId, fields.WinnerIndex, fields.PlayerIds, fields.PlayerWinnerId)
        {
            Id = id;
        }
    }
}