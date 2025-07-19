using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Data;
using JetBrains.Annotations;

namespace Epic.Core.Services.Users
{
    [UsedImplicitly]
    public class DefaultUsersService : IUsersService
    {
        private IUsersRepository UsersRepository { get; }

        public DefaultUsersService([NotNull] IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task<IUserObject> GetUserById(Guid userId)
        {
            var userEntity = await UsersRepository.GetUserByIdAsync(userId);
            return ToUserObject(userEntity);
        }

        public async Task<IUserObject[]> GetUsersByIds(Guid[] userIds)
        {
            var entities = await UsersRepository.GetUsersByIdsAsync(userIds);
            return entities.Select(ToUserObject).ToArray();
        }

        public async Task<IUserObject> GetUserByHashAsync(string hash)
        {
            var userEntity = await UsersRepository.GetUserByHashAsync(hash);
            var userObject = ToUserObject(userEntity);
            return userObject;
        }

        public async Task<IUserObject> CreateComputerUser()
        {
            var id = Guid.NewGuid();
            var name = "$computer:" + id;
            var userEntity = await UsersRepository.CreateUserAsync(name, name, true);
            return ToUserObject(userEntity);
        }

        private static IUserObject ToUserObject(IUserEntity userEntity)
        {
            return new MutableUserObject
            {
                Id = userEntity.Id,
                Hash = userEntity.Hash,
                Name = userEntity.Name,
                IsSystem = userEntity.IsSystem,
                IsBlocked = userEntity.IsBlocked,
            };
        }
    }
}