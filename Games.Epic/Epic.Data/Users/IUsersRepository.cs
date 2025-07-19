using System;
using System.Threading.Tasks;

namespace Epic.Data
{
    public interface IUsersRepository : IRepository
    {
        Task<IUserEntity> GetUserByIdAsync(Guid id);
        Task<IUserEntity[]> GetUsersByIdsAsync(Guid[] ids);
        Task<IUserEntity> GetUserByHashAsync(string hash);
        
        Task<IUserEntity> CreateUserAsync(string name, string hash, bool isSystem = false);
        Task DeleteUserAsync(Guid id);
    }
}