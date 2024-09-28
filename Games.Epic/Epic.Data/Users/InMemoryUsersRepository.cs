using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data
{
    [UsedImplicitly]
    public class InMemoryUsersRepository : IUsersRepository
    {
        public string Name => nameof(InMemoryUsersRepository);
        public string EntityName => "User";
        
        private readonly Dictionary<Guid, IUserEntity> _users = new Dictionary<Guid, IUserEntity>();

        public InMemoryUsersRepository()
        {
            _users.Add(Guid.Empty, new UserEntity
            {
                Id = Guid.NewGuid(),
                IsBlocked = false,
                Name = "admin",
                Type = UserEntityType.Player,
                Hash = "Basic YWRtaW46",
            });
        }
        
        public Task<IUserEntity> GetUserByIdAsync(Guid id)
        {
            // Find the user by ID in the dictionary
            if (_users.TryGetValue(id, out var user))
            {
                return Task.FromResult(user);
            } else 
                throw new EntityNotFoundException(this, id.ToString());
        }

        public Task<IUserEntity> GetUserByHashAsync(string hash)
        {
            // Find the user by hash
            var user = _users.Values.FirstOrDefault(u => u.Hash == hash);
            if (user == null)
                throw new EntityNotFoundException(this, hash);
            return Task.FromResult(user);
        }
        
        public Task<IUserEntity> CreateUserAsync(string name, string hash, UserEntityType type)
        {
            var entity = new UserEntity
            {
                Id = Guid.NewGuid(), 
                Name = name,
                Hash = hash,
                IsBlocked = false,
                Type = type,
            };
            _users[entity.Id] = entity;
            return Task.FromResult((IUserEntity)entity);
        }

        public Task DeleteUserAsync(Guid id)
        {
            _users.Remove(id);
            return Task.CompletedTask;
        }
    }
}