using System;
using System.Threading.Tasks;
using Epic.Core.Objects;

namespace Epic.Core
{
    public interface IUsersService
    {
        Task<IUserObject> GetUserById(Guid userId);
        Task<IUserObject> GetUserByHashAsync(string hash);

        Task<IUserObject> CreateComputerUser();
    }
}