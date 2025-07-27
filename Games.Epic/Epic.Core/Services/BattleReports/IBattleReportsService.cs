using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.BattleReports
{
    public interface IBattleReportsService
    {
        Task<IBattleReportObject> GetById(Guid id);
    }
}