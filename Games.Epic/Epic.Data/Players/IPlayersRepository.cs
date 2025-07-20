using System;
using System.Threading.Tasks;

namespace Epic.Data.Players
{
    public interface IPlayersRepository : IRepository
    {
        Task<IPlayerEntity> GetById(Guid playerId);
        Task<IPlayerEntity> GetByName(string name);
        Task<IPlayerEntity[]> GetByUserId(Guid userId);
        Task Update(IPlayerEntity entity);
        Task<IPlayerEntity> Create(IPlayerEntityFields fields);
        Task SetDefeated(Guid[] playerIds);
        Task DayIncrement(Guid[] playerIds);
        Task SetGenerationInProgress(Guid[] playerIds, bool isGenerationInProgress);
    }
}