using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.Users;
using Epic.Data.Players;
using JetBrains.Annotations;

namespace Epic.Core.Services.Players
{
    [UsedImplicitly]
    public class DefaultPlayersService : IPlayersService
    {
        public IPlayersRepository PlayersRepository { get; }
        public IUsersService UsersService { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IHeroesService HeroesService { get; }

        public DefaultPlayersService(
            [NotNull] IPlayersRepository playersRepository,
            [NotNull] IUsersService usersService,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IHeroesService heroesService)
        {
            PlayersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
        }

        public async Task<IPlayerObject> GetByIdAndUserId(Guid userId, Guid playerId)
        {
            var user = await UsersService.GetUserById(userId);
            var player = await PlayersRepository.GetById(playerId);
            if (user.Id != player.UserId) 
                throw new InvalidOperationException("Player does not belong to this user");

            return await FromEntity(player);
        }

        public async Task<IPlayerObject> CreatePlayer(Guid userId, string name, PlayerObjectType playerObjectType)
        {
            var user = await UsersService.GetUserById(userId);
            
            var supplyContainer = await UnitsContainersService.Create(30, Guid.Empty);
            var entity = await PlayersRepository.Create(new MutablePlayerEntityFields
            {
                Day = 0,
                Name = name,
                PlayerType = playerObjectType.ToEntity(),
                UserId = user.Id,
                GenerationInProgress = false,
                SupplyContainerId = supplyContainer.Id,
            });
            supplyContainer = await UnitsContainersService.ChangeOwner(supplyContainer, entity.Id);
            
            return await FromEntity(entity, supplyContainer);
        }

        public async Task<IPlayerObject> GetById(Guid playerId)
        {
            var playerEntity = await PlayersRepository.GetById(playerId);
            return await FromEntity(playerEntity);
        }

        public async Task<IPlayerObject[]> GetAllByUserId(Guid userId)
        {
            var entities = await PlayersRepository.GetByUserId(userId);
            return await Task.WhenAll(entities.Select(x => FromEntity(x)));
        }

        public async Task<IPlayerObject[]> GetByIds(Guid[] playerIds)
        {
            var entities = await PlayersRepository.GetByIds(playerIds);
            return await Task.WhenAll(entities.Select(x => FromEntity(x)));
        }

        public Task DayIncrement(Guid[] playerIds)
        {
           return PlayersRepository.DayIncrement(playerIds);
        }

        public Task SetGenerationInProgress(Guid playerId, bool generationInProgress)
        {
            return PlayersRepository.SetGenerationInProgress(new[] { playerId }, generationInProgress);
        }

        public Task SetActiveHero(Guid playerId, Guid heroId)
        {
            return PlayersRepository.SetActiveHero(playerId, heroId);
        }

        private async Task<IPlayerObject> FromEntity(IPlayerEntity playerEntity,
            IUnitsContainerObject supplyContainer = null,
            IHeroObject heroObject = null)
        {
            var mutableObject = MutablePlayerObject.FromEntity(playerEntity);
            mutableObject.Supply = supplyContainer ?? await UnitsContainersService.GetById(mutableObject.SupplyContainerId);
            mutableObject.ActiveHero = heroObject;
            if (mutableObject.ActiveHero == null && mutableObject.ActiveHeroId.HasValue)
                mutableObject.ActiveHero = await HeroesService.GetById(mutableObject.ActiveHeroId.Value);
            return mutableObject;
        }
    }
}