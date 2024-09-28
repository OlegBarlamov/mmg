using System;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Data;
using JetBrains.Annotations;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultUsersService : IUsersService
    {
        private IUsersRepository UsersRepository { get; }

        public DefaultUsersService([NotNull] IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task<IUserObject> GetUserByHashAsync(string hash)
        {
            var userEntity = await UsersRepository.GetUserByHashAsync(hash);
            var userObject = ToUserObject(userEntity);
            return userObject;
        }

        private static IUserObject ToUserObject(IUserEntity userEntity)
        {
            UserObjectType userType = userEntity.Type switch
            {
                UserEntityType.Computer => UserObjectType.Computer,
                UserEntityType.Player => UserObjectType.Player,
                _ => throw new ArgumentOutOfRangeException(nameof(userEntity.Type), userEntity.Type.ToString(), null)
            };
            return new MutableUserObject
            {
                Id = userEntity.Id,
                Hash = userEntity.Hash,
                Name = userEntity.Name,
                Type = userType,
                IsBlocked = userEntity.IsBlocked,
            };
        }
    }
}