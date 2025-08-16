using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Players;
using Epic.Core.Services.UnitTypes;
using Epic.Data.BattleReports;
using Epic.Data.BattleUnits;
using Epic.Data.GlobalUnits;
using JetBrains.Annotations;

namespace Epic.Core.Services.BattleReports
{
    [UsedImplicitly]
    public class DefaultBattleReportsService : IBattleReportsService
    {
        public IBattleReportsRepository Repository { get; }
        public IPlayersService PlayersService { get; }
        public IBattleUnitsRepository BattleUnitsRepository { get; }
        public IUnitTypesService UnitTypesService { get; }
        public IGlobalUnitsRepository GlobalUnitsRepository { get; }

        public DefaultBattleReportsService(
            [NotNull] IBattleReportsRepository repository,
            [NotNull] IPlayersService playersService,
            [NotNull] IBattleUnitsRepository battleUnitsRepository,
            [NotNull] IUnitTypesService unitTypesService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            BattleUnitsRepository = battleUnitsRepository ?? throw new ArgumentNullException(nameof(battleUnitsRepository));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
        }
        
        public async Task<IBattleReportObject> GetById(Guid id)
        {
            var entity = await Repository.GetById(id);
            var report = MutableBattleReportObject.FromEntity(entity);
            await FillReportObject(report);
            return report;
        }
        
        private async Task FillReportObject(MutableBattleReportObject report)
        {
            report.Players = await PlayersService.GetByIds(report.PlayerIds);
            report.Units = await GetUnits(report.BattleId);
        }

        private async Task<IReadOnlyList<IBattleReportUnitObject>> GetUnits(Guid battleId)
        {
            var unitEntities = await BattleUnitsRepository.GetByBattleId(battleId);
            var unitsToGlobalMap = unitEntities.ToDictionary(x => x.Id, x => x.GlobalUnitId);
            var globalUnits = await GlobalUnitsRepository.FetchUnitsByIds(unitsToGlobalMap.Values);
            var globalUnitsToTypesMap = globalUnits.ToDictionary(x => x.Id, x => x.TypeId);
            var types = await UnitTypesService.GetUnitTypesByIdsAsync(globalUnitsToTypesMap.Values
                .Distinct()
                .ToArray()
            );
            var typesMap = types.ToDictionary(x => x.Id, x => x);

            return unitEntities.Select(x => MutableBattleReportUnitObject.FromEntity(x,
                typesMap[globalUnitsToTypesMap[unitsToGlobalMap[x.Id]]])).ToArray();
        }
    }
}