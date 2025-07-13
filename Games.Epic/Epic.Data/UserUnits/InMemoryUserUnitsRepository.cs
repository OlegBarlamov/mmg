using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.UserUnits
{
    [UsedImplicitly]
    public class InMemoryUserUnitsRepository : IUserUnitsRepository
    {
        public string Name => nameof(InMemoryUserUnitsRepository);
        public string EntityName => "UserUnit";
        
        private readonly List<UserUnitEntity> _userUnits = new List<UserUnitEntity>();
        
        public Task<IUserUnitEntity[]> GetUnitsByUserAsync(Guid userId)
        {
            var units = _userUnits.Where(unit => unit.UserId == userId).ToArray<IUserUnitEntity>();
            return Task.FromResult(units);
        }
        
        public Task<IUserUnitEntity[]> GetAliveUnitsByUserAsync(Guid userId)
        {
            var aliveUnits = _userUnits
                .Where(unit => unit.UserId == userId && unit.IsAlive)
                .ToArray<IUserUnitEntity>();
            return Task.FromResult(aliveUnits);
        }

        public Task<IUserUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            return Task.FromResult(_userUnits.Where(unit => ids.Contains(unit.Id)).ToArray<IUserUnitEntity>());
        }

        public Task<IUserUnitEntity> CreateUserUnit(Guid typeId, int count, Guid userId, bool isAlive)
        {
            var entity = new UserUnitEntity
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                Count = count,
                UserId = userId,
                IsAlive = isAlive,
            };
            
            _userUnits.Add(entity);
            
            return Task.FromResult((IUserUnitEntity)entity);
        }

        public Task Update(IUserUnitEntity[] entities)
        {
            entities.ForEach(entity =>
            {
                var target = _userUnits.First(x => x.Id == entity.Id);
                target.Count = entity.Count;
                target.UserId = entity.UserId;
                target.IsAlive = entity.IsAlive;
                target.TypeId = entity.TypeId;
            });
            return Task.CompletedTask;
        }
    }
}