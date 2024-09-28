using System.Threading.Tasks;
using Epic.Core.Objects;

namespace Epic.Core
{
    public interface IUsersService
    {
        Task<IUserObject> GetUserByHashAsync(string hash);
    }
}