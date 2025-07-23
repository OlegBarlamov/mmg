using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.Heroes;
using JetBrains.Annotations;

namespace Epic.Core.Services.Heroes
{
    [UsedImplicitly]
    public class DefaultHeroesService : IHeroesService
    {
        public IHeroEntitiesRepository HeroEntitiesRepository { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IGlobalUnitsService GlobalUnitsService { get; }

        public DefaultHeroesService(
            [NotNull] IHeroEntitiesRepository heroEntitiesRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IGlobalUnitsService globalUnitsService)
        {
            HeroEntitiesRepository = heroEntitiesRepository ?? throw new ArgumentNullException(nameof(heroEntitiesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
        }
        
        public async Task<IHeroObject> GetById(Guid id)
        {
            var entity = await HeroEntitiesRepository.GetById(id);
            var hero = MutableHeroObject.FromEntity(entity);
            await FillHeroObject(hero);
            return hero;
        }

        public async Task<IHeroObject> CreateNew(string name, Guid playerId, bool setActive = false)
        {
            var container = await UnitsContainersService.Create(7, playerId);
            var entity = await HeroEntitiesRepository.CreateForPlayer(playerId, new MutableHeroEntityFields
            {
                Name = name,
                ArmyContainerId = container.Id,
                IsKilled = false,
                Level = 1,
                Experience = 0,
            }, playerId);
            var hero = MutableHeroObject.FromEntity(entity);
            await FillHeroObject(hero);
            return hero;
        }

        public async Task<IReadOnlyList<IHeroObject>> GetByPlayerId(Guid playerId)
        {
            var entities = await HeroEntitiesRepository.GetByPlayerId(playerId);
            var heroes = entities.Select(MutableHeroObject.FromEntity).ToArray();
            await Task.WhenAll(heroes.Select(FillHeroObject));
            return heroes;
        }

        private async Task FillHeroObject(MutableHeroObject heroObject)
        {
            heroObject.ArmyContainer = await UnitsContainersService.GetById(heroObject.ArmyContainerId);
            heroObject.HasAliveUnits = await GlobalUnitsService.HasAliveUnits(heroObject.ArmyContainerId);
        }
    }
}