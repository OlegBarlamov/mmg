using System;
using System.Threading.Tasks;
using Epic.Core.Objects;

namespace Epic.Core.Services.Users
{
    public interface IUsersService
    {
        Task<IUserObject> GetUserById(Guid userId);
        Task<IUserObject[]> GetUsersByIds(Guid[] userId);
        Task<IUserObject> GetUserByHashAsync(string hash);

        Task<IUserObject> CreateComputerUser();
    }
}