using System;
using System.Threading.Tasks;
using Epic.Core.Objects;

namespace Epic.Core.Services.Players
{
    public interface IPlayersService
    {
        Task<IPlayerObject> GetByIdAndUserId(Guid userId, Guid playerId);
        Task<IPlayerObject> CreatePlayer(Guid userId, string name, PlayerObjectType playerObjectType);
        Task<IPlayerObject> CreateComputerPlayer(IUserObject user, Guid humanPlayerId);
        Task<IPlayerObject> GetById(Guid playerId);
        Task<IPlayerObject[]> GetAllByUserId(Guid userId);
    }
}
