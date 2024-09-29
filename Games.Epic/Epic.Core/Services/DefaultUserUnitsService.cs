using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Data.UserUnits;
using JetBrains.Annotations;

namespace Epic.Core
{
    public class DefaultUserUnitsService : IUserUnitsService
    {
        public IUserUnitsRepository UserUnitsRepository { get; }

        public DefaultUserUnitsService([NotNull] IUserUnitsRepository userUnitsRepository)
        {
            UserUnitsRepository = userUnitsRepository ?? throw new ArgumentNullException(nameof(userUnitsRepository));
        }
        
        public async Task<IUserUnitObject[]> GetAliveUnitsByUserAsync(Guid userId)
        {
            var aliveUsersEntities = await UserUnitsRepository.GetAliveUnitsByUserAsync(userId);
            return aliveUsersEntities.Select(ToUserUnitObject).ToArray();
        }

        private static IUserUnitObject ToUserUnitObject(IUserUnitEntity entity)
        {
            return new MutableUserUnitObject
            {
                Id = entity.Id,
                Count = entity.Count,
                IsAlive = entity.IsAlive,
                UserId = entity.UserId,
                TypeId = entity.TypeId,
            };
        }
    }
}