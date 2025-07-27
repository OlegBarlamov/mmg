using System;
using System.Threading.Tasks;

namespace Epic.Data.BattleReports
{
    public interface IBattleReportsRepository : IRepository
    {
        Task<IBattleReportEntity> GetById(Guid id);
        Task<IBattleReportEntity> GetByBattleId(Guid battleId);
        Task<IBattleReportEntity[]> GetAllByPlayerId(Guid playerId);

        Task<IBattleReportEntity> Create(IBattleReportFields fields);
    }
}