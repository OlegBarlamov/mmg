using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects;
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

        public DefaultPlayersService([NotNull] IPlayersRepository playersRepository, [NotNull] IUsersService usersService)
        {
            PlayersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        public async Task<IPlayerObject> GetByIdAndUserId(Guid userId, Guid playerId)
        {
            var user = await UsersService.GetUserById(userId);
            var player = await PlayersRepository.GetById(playerId);
            if (user.Id != player.UserId) 
                throw new InvalidOperationException("Player does not belong to this user");

            return MutablePlayerObject.FromEntity(player);
        }

        public async Task<IPlayerObject> CreatePlayer(Guid userId, string name, PlayerObjectType playerObjectType)
        {
            var user = await UsersService.GetUserById(userId);
            var entity = await PlayersRepository.Create(new MutablePlayerEntityFields
            {
                Day = 0,
                IsDefeated = false,
                Name = name,
                PlayerType = playerObjectType.ToEntity(),
                UserId = user.Id,
            });
            return MutablePlayerObject.FromEntity(entity);
        }

        public Task<IPlayerObject> CreateComputerPlayer(IUserObject user, Guid humanPlayerId)
        {
            if (!user.IsSystem) 
                throw new InvalidOperationException("Computer player can only be created on system users.");

            return CreatePlayer(user.Id, $"npc_for_{humanPlayerId}", PlayerObjectType.Computer);
        }

        public async Task<IPlayerObject> GetComputerPlayer(Guid humanPlayerId)
        {
            var playerEntity = await PlayersRepository.GetByName($"npc_for_{humanPlayerId}");
            return MutablePlayerObject.FromEntity(playerEntity);
        }

        public async Task<IPlayerObject> GetById(Guid playerId)
        {
            var playerEntity = await PlayersRepository.GetById(playerId);
            return MutablePlayerObject.FromEntity(playerEntity);
        }

        public async Task<IPlayerObject[]> GetAllByUserId(Guid userId)
        {
            var entities = await PlayersRepository.GetByUserId(userId);
            return entities.Select(MutablePlayerObject.FromEntity).ToArray<IPlayerObject>();
        }

        public Task SetDefeated(Guid[] playerIds)
        {
            return PlayersRepository.SetDefeated(playerIds);
        }

        public Task DayIncrement(Guid[] playerIds)
        {
           return PlayersRepository.DayIncrement(playerIds);
        }

        public Task SetGenerationInProgress(Guid playerId, bool generationInProgress)
        {
            return PlayersRepository.SetGenerationInProgress(new[] { playerId }, generationInProgress);
        }
    }
}