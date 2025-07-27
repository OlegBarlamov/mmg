using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.BattleReports
{
    [UsedImplicitly]
    public class InMemoryBattleReportsRepository : IBattleReportsRepository
    {
        public string Name => nameof(InMemoryBattleReportsRepository);
        public string EntityName => "BattleReport";
        
        private readonly List<IBattleReportEntity> _reports = new List<IBattleReportEntity>();
        
        public Task<IBattleReportEntity> GetById(Guid id)
        {
            return Task.FromResult(_reports.First(x => x.Id == id));
        }

        public Task<IBattleReportEntity> GetByBattleId(Guid battleId)
        {
            return Task.FromResult(_reports.First(x => x.BattleId == battleId));
        }

        public Task<IBattleReportEntity[]> GetAllByPlayerId(Guid playerId)
        {
            return Task.FromResult(_reports.Where(x => x.PlayerIds.Contains(playerId)).ToArray());
        }

        public Task<IBattleReportEntity> Create(IBattleReportFields fields)
        {
            var entity = new MutableBattleReportEntity(Guid.NewGuid(), fields);
            _reports.Add(entity);
            return Task.FromResult<IBattleReportEntity>(entity);
        }
    }
}