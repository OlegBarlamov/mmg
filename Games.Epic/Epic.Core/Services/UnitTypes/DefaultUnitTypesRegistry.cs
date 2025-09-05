using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Data.UnitTypes;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Core.Services.UnitTypes
{
    [UsedImplicitly]
    public class DefaultUnitTypesRegistry : IUnitTypesRegistry
    {
        public IReadOnlyList<IUnitTypeEntity> AllOrderedByValue => _orderedUnitTypes;
        public IReadOnlyList<IUnitTypeEntity> ToTrainOrderedByValue => _toTrainOrderedUnitTypes;

        private IUnitTypesRepository UnitTypesRepository { get; }
        
        private readonly List<IUnitTypeEntity> _orderedUnitTypes = new List<IUnitTypeEntity>();
        private readonly List<IUnitTypeEntity> _toTrainOrderedUnitTypes = new List<IUnitTypeEntity>();
        private readonly Dictionary<Guid, IUnitTypeEntity> _unitTypesByIds = new Dictionary<Guid, IUnitTypeEntity>();
        private readonly Dictionary<Guid, List<Guid>> _upgrades = new Dictionary<Guid, List<Guid>>();
        
        public DefaultUnitTypesRegistry([NotNull] IUnitTypesRepository repository)
        {
            UnitTypesRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public IUnitTypeEntity ById(Guid typeId)
        {
            return _unitTypesByIds[typeId];
        }

        public IReadOnlyList<IUnitTypeEntity> FindUpgradesFor(Guid typeId)
        {
            return _upgrades.TryGetValue(typeId, out var upgrades) 
                ? upgrades.Select(ById).ToArray() 
                : Array.Empty<IUnitTypeEntity>();
        }

        public async Task Load(CancellationToken cancellationToken)
        {
            _unitTypesByIds.Clear();
            _upgrades.Clear();
            _orderedUnitTypes.Clear();
            
            
            var allUnits = await UnitTypesRepository.GetAll();
            _orderedUnitTypes.AddRange(allUnits);
            _orderedUnitTypes.Sort((x, y) => x.Value.CompareTo(y.Value));
            
            
            allUnits.ForEach(x =>
            {
                _unitTypesByIds.Add(x.Id, x);
                if (x.ToTrainAmount > 0)
                    _toTrainOrderedUnitTypes.Add(x);
                
                foreach (var upgradeForId in x.UpgradeForUnitTypeIds)
                {
                    if (!_upgrades.TryGetValue(upgradeForId, out var existedUpgradeFor))
                    {
                        existedUpgradeFor = new List<Guid>();
                        _upgrades.Add(upgradeForId, existedUpgradeFor);
                    }
                    
                    existedUpgradeFor.Add(x.Id);
                }
            });
        }
    }
}