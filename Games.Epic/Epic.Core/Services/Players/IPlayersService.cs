using System;
using System.Threading.Tasks;
using Epic.Core.Services.Users;

namespace Epic.Core.Services.Players
{
    public interface IPlayersService
    {
        Task<IPlayerObject> GetByIdAndUserId(Guid userId, Guid playerId);
        Task<IPlayerObject> CreatePlayer(Guid userId, string name, PlayerObjectType playerObjectType);
        Task<IPlayerObject> GetById(Guid playerId);
        Task<IPlayerObject[]> GetAllByUserId(Guid userId);
        Task<IPlayerObject[]> GetByIds(Guid[] playerIds);
        Task DayIncrement(Guid[] playerIds);
        Task SetGenerationInProgress(Guid playerId, bool generationInProgress);
        Task SetActiveHero(Guid playerId, Guid heroId);
    }
}
