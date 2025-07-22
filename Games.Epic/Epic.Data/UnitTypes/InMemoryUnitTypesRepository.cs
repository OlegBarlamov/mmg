using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data.UnitTypes
{
    [UsedImplicitly]
    public class InMemoryUnitTypesRepository : IUnitTypesRepository
    {
        public string Name => nameof(InMemoryUnitTypesRepository);
        public string EntityName => "UnitType";
        
        private readonly List<IUnitTypeEntity> _unitTypes = new List<IUnitTypeEntity>();
        
        public Task<IUnitTypeEntity> GetById(Guid id)
        {
            var unitType = _unitTypes.FirstOrDefault(u => u.Id == id);
            if (unitType == null)
                throw new EntityNotFoundException(this, id.ToString());
            
            return Task.FromResult(unitType);
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

        public Task<IUnitTypeEntity[]> GetAll()
        {
            return Task.FromResult(_unitTypes.ToArray());
        }
    }
}