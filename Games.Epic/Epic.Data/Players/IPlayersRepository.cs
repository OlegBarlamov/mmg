using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.Players
{
    public interface IPlayersRepository : IRepository
    {
        Task<IPlayerEntity> GetById(Guid playerId);
        Task<IPlayerEntity> FindByName(string playerName);
        Task<IPlayerEntity[]> GetByIds(IReadOnlyList<Guid> playerId);
        Task<IPlayerEntity[]> GetByUserId(Guid userId);
        Task Update(IPlayerEntity entity);
        Task<IPlayerEntity> Create(IPlayerEntityFields fields);
        Task DayIncrement(IReadOnlyList<Guid> playerIds);
        Task SetGenerationInProgress(Guid[] playerIds, bool isGenerationInProgress);
        Task SetActiveHero(Guid playerId, Guid heroId);
    }
}