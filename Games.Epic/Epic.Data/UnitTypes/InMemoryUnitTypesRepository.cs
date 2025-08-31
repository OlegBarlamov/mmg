using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.UnitTypes
{
    [UsedImplicitly]
    public class InMemoryUnitTypesRepository : IUnitTypesRepository
    {
        public string Name => nameof(InMemoryUnitTypesRepository);
        public string EntityName => "UnitType";
        
        private readonly List<UnitTypeEntity> _unitTypes = new List<UnitTypeEntity>();
        
        public Task<IUnitTypeEntity> GetById(Guid id)
        {
            var unitType = _unitTypes.FirstOrDefault(u => u.Id == id);
            if (unitType == null)
                throw new EntityNotFoundException(this, id.ToString());
            
            return Task.FromResult<IUnitTypeEntity>(unitType);
        }

        public Task<IUnitTypeEntity> GetByName(string name)
        {
            return Task.FromResult<IUnitTypeEntity>(_unitTypes.FirstOrDefault(u => u.Name == name));
        }

        public Task<IReadOnlyCollection<IUnitTypeEntity>> FetchByIds(IEnumerable<Guid> ids)
        {
            var unitTypes = ids.Select(id => _unitTypes.FirstOrDefault(u => u.Id == id)).ToArray();
            return Task.FromResult<IReadOnlyCollection<IUnitTypeEntity>>(unitTypes);
        }

        public Task<IUnitTypeEntity> CreateUnitType(Guid id, UnitTypeProperties properties)
        {
            var entity = UnitTypeEntity.FromProperties(id, properties);
            
            _unitTypes.Add(entity);
            
            return Task.FromResult((IUnitTypeEntity)entity);
        }

        public Task<IUnitTypeEntity[]> CreateBatch(IEnumerable<UnitTypeProperties> properties)
        {
            var entities = properties.Select(x => UnitTypeEntity.FromProperties(Guid.NewGuid(), x))
                .ToArray();
            
            _unitTypes.AddRange(entities);
            
            return Task.FromResult(entities.ToArray<IUnitTypeEntity>());
        }

        public Task UpdateBatch(IEnumerable<IUnitTypeEntity> updatedEntities)
        {
            updatedEntities.ForEach(x => _unitTypes.First(y => x.Id == y.Id).FillProperties(x));
            return Task.CompletedTask;
        }

        public Task<IUnitTypeEntity[]> GetAll()
        {
            return Task.FromResult(_unitTypes.ToArray<IUnitTypeEntity>());
        }

        public Task<IUnitTypeEntity[]> GetUpgradesFor(Guid unitTypeId)
        {
            var result = _unitTypes.Where(x => x.UpgradeForUnitTypeIds.Contains(unitTypeId));
            return Task.FromResult(result.ToArray<IUnitTypeEntity>());
        }
    }
}