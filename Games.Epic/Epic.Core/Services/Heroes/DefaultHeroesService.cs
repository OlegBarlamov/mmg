using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.Artifacts;
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
        public IHeroesLevelsCalculator HeroesLevelsCalculator { get; }
        public IArtifactsService ArtifactsService { get; }

        public DefaultHeroesService(
            [NotNull] IHeroEntitiesRepository heroEntitiesRepository,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IHeroesLevelsCalculator heroesLevelsCalculator,
            [NotNull] IArtifactsService artifactsService)
        {
            HeroEntitiesRepository = heroEntitiesRepository ?? throw new ArgumentNullException(nameof(heroEntitiesRepository));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            HeroesLevelsCalculator = heroesLevelsCalculator ?? throw new ArgumentNullException(nameof(heroesLevelsCalculator));
            ArtifactsService = artifactsService ?? throw new ArgumentNullException(nameof(artifactsService));
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
            var container = await UnitsContainersService.Create(3, playerId);
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

        public async Task GiveExperience(Guid heroId, int experience)
        {
            var heroObject = await GetById(heroId);
            var gainResult = HeroesLevelsCalculator.GiveExperience(heroObject, experience);

            var mutableHeroObject = MutableHeroObject.CopyFrom(heroObject);
            mutableHeroObject.Experience += gainResult.ExperienceGain;
            mutableHeroObject.Level += gainResult.LevelsGain;
            mutableHeroObject.Attack += gainResult.AttacksGain;
            mutableHeroObject.Defense += gainResult.DefenseGain;

            await HeroEntitiesRepository.Update(mutableHeroObject.Id, mutableHeroObject);

            if (gainResult.ArmySlotsGain != 0)
            {
                var newCapacity = Math.Min(11, heroObject.ArmyContainer.Capacity + gainResult.ArmySlotsGain);
                await UnitsContainersService.ChangeCapacity(heroObject.ArmyContainer, newCapacity);
            }
        }

        public async Task AddAttack(Guid heroId, int attackAmount)
        {
            if (attackAmount <= 0)
                return;
            
            var heroObject = await GetById(heroId);
            var mutableHeroObject = MutableHeroObject.CopyFrom(heroObject);
            mutableHeroObject.Attack += attackAmount;
            await HeroEntitiesRepository.Update(mutableHeroObject.Id, mutableHeroObject);
        }

        public async Task AddDefense(Guid heroId, int defenseAmount)
        {
            if (defenseAmount <= 0)
                return;
            
            var heroObject = await GetById(heroId);
            var mutableHeroObject = MutableHeroObject.CopyFrom(heroObject);
            mutableHeroObject.Defense += defenseAmount;
            await HeroEntitiesRepository.Update(mutableHeroObject.Id, mutableHeroObject);
        }

        private async Task FillHeroObject(MutableHeroObject heroObject)
        {
            heroObject.ArmyContainer = await UnitsContainersService.GetById(heroObject.ArmyContainerId);
            heroObject.HasAliveUnits = await GlobalUnitsService.HasAliveUnits(heroObject.ArmyContainerId);
            heroObject.Artifacts = await ArtifactsService.GetByHeroId(heroObject.Id);
        }
    }
}