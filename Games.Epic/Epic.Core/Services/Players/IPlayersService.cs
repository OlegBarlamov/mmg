using System;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Services.Users;

namespace Epic.Core.Services.Players
{
    public interface IPlayersService
    {
        Task<IPlayerObject> GetByIdAndUserId(Guid userId, Guid playerId);
        Task<IPlayerObject> CreatePlayer(Guid userId, string name, PlayerObjectType playerObjectType);
        Task<IPlayerObject> CreateComputerPlayer(IUserObject user, Guid humanPlayerId);
        Task<IPlayerObject> GetComputerPlayer(Guid humanPlayerId);
        Task<IPlayerObject> GetById(Guid playerId);
        Task<IPlayerObject[]> GetAllByUserId(Guid userId);
        Task SetDefeated(Guid[] playerIds);
        Task DayIncrement(Guid[] playerIds);
        Task SetGenerationInProgress(Guid playerId, bool generationInProgress);
    }
}
