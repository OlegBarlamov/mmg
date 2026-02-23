using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core.Services.Heroes
{
    public interface IHeroesService
    {
        Task<IHeroObject> GetById(Guid id);
        Task<IHeroObject> CreateNew(string name, Guid playerId, bool setActive = false);
        Task<IReadOnlyList<IHeroObject>> GetByPlayerId(Guid playerId);
        Task GiveExperience(Guid heroId, int experience);
        Task AddAttack(Guid heroId, int attackAmount);
        Task AddDefense(Guid heroId, int defenseAmount);
        Task RestoreMana(Guid heroId);
        Task SpendMana(Guid heroId, int amount);
        Task AddKnownMagic(Guid heroId, Guid magicTypeId);
        Task AddKnownMagicByKey(Guid heroId, string magicTypeKey);
    }
}