using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Objects;
using Epic.Core.Services.Players;
using Epic.Data.BattleReports;
using JetBrains.Annotations;

namespace Epic.Core.Services.BattleReports
{
    public interface IBattleReportObject : IGameObject<IBattleReportEntity>
    {
        Guid Id { get; }
        Guid BattleId { get; }
        int? WinnerIndex { get; }
        Guid? PlayerWinnerId { get; }

        Guid[] PlayerIds { get; }

        IReadOnlyList<IPlayerObject> Players { get; }
        IReadOnlyList<IBattleReportUnitObject> Units { get; }

        IPlayerObject GetWinnerOrNull();
        
        int GetPlayerIndex(Guid playerId);
        
        bool HasWinner { get; }
    }

    public class MutableBattleReportObject : IBattleReportObject
    {
        public Guid Id { get; set; }
        public Guid BattleId { get; set; }
        public int? WinnerIndex { get; set; }
        public Guid? PlayerWinnerId { get; set; }
        
        public Guid[] PlayerIds { get; set; }
        
        public IReadOnlyList<IPlayerObject> Players { get; set; }
        public IReadOnlyList<IBattleReportUnitObject> Units { get; set; }

        private MutableBattleReportObject(Guid id)
        {
            Id = id;
        }

        public static MutableBattleReportObject FromEntity(IBattleReportEntity entity)
        {
            return new MutableBattleReportObject(entity.Id)
            {
                BattleId = entity.BattleId,
                WinnerIndex = entity.WinnerIndex,
                PlayerWinnerId = entity.PlayerWinnerId,
                PlayerIds = entity.PlayerIds,
            };
        }
        
        public IPlayerObject GetWinnerOrNull()
        {
            return PlayerWinnerId.HasValue ? Players.First(x => x.Id == PlayerWinnerId) : null;
        }

        public int GetPlayerIndex(Guid playerId)
        {
            var index = Array.IndexOf(PlayerIds, playerId);
            if (index < 0)
                throw new IndexOutOfRangeException();

            return index + 1;
        }

        public bool HasWinner => WinnerIndex.HasValue;

        public IBattleReportEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}