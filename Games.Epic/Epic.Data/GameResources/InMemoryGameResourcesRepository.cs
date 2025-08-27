using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.GameResources
{
    [UsedImplicitly]
    public class InMemoryGameResourcesRepository : IGameResourcesRepository
    {
        public Guid GoldResourceId => Guid.Empty;
        public string Name => nameof(InMemoryGameResourcesRepository);
        public string EntityName => "GameResources";

        private readonly List<MutableGameResourceEntity> _resources = new List<MutableGameResourceEntity>();
        private readonly List<MutableResourceByPlayerEntity> _resourceByPlayers = new List<MutableResourceByPlayerEntity>();

        public Task<IGameResourceEntity> GetById(Guid id)
        {
            return Task.FromResult<IGameResourceEntity>(_resources.First(x => x.Id == id));
        }

        public Task<IReadOnlyList<IGameResourceEntity>> GetByIds(IEnumerable<Guid> ids)
        {
            return Task.FromResult<IReadOnlyList<IGameResourceEntity>>(
                ids.Select(x => _resources.First(y => y.Id == x))
                .ToArray());
        }

        public Task<IReadOnlyDictionary<string, IGameResourceEntity>> GetAllResourcesByKeys()
        {
            return Task.FromResult<IReadOnlyDictionary<string, IGameResourceEntity>>(
                _resources.ToDictionary(x => x.Key, x => (IGameResourceEntity)x));
        }

        public Task<IGameResourceEntity> Create(string key, string name, string iconUrl, int price)
        {
            var entity = new MutableGameResourceEntity
            {
                Id = key == PredefinedResourcesKeys.Gold ? GoldResourceId : Guid.NewGuid(),
                Name = name,
                Key = key,
                IconUrl = iconUrl,
                Price = price,
            };
            _resources.Add(entity);
            return Task.FromResult<IGameResourceEntity>(entity);
        }

        public Task<IGameResourceEntity[]> Create(IEnumerable<IGameResourceEntity> entities)
        {
            var newEntities = entities.Select(x => new MutableGameResourceEntity
            {
                Id = Guid.NewGuid(),
                IconUrl = x.IconUrl,
                Key = x.Key,
                Name = x.Name,
                Price = x.Price,
            }).ToArray();
            
            _resources.AddRange(newEntities);
            
            return Task.FromResult(newEntities.ToArray<IGameResourceEntity>());
        }

        public Task<IReadOnlyList<ResourceAmount>> GetAllResourcesByPlayer(Guid playerId)
        {
            return Task.FromResult<IReadOnlyList<ResourceAmount>>(_resources.Select(resource =>
            {
                var byPlayerEntity = _resourceByPlayers.FirstOrDefault(x => x.ResourceId == resource.Id && x.PlayerId == playerId);
                var amount = byPlayerEntity?.Amount ?? 0;
                return ResourceAmount.Create(resource, amount);
            }).ToArray());
        }

        public Task<ResourceAmount> GetResourceByPlayer(Guid resourceId, Guid playerId)
        {
            var resource = _resources.First(x => x.Id == resourceId);
            var byPlayer = _resourceByPlayers.FirstOrDefault(x => x.PlayerId == playerId && x.ResourceId == resourceId);
            var amount = byPlayer?.Amount ?? 0;
            return Task.FromResult(ResourceAmount.Create(resource, amount));
        }

        public Task<ResourceAmount> GiveResource(Guid resourceId, Guid playerId, int delta)
        {
            var entity = GetResourceByPlayerInternal(resourceId, playerId);
            entity.Amount += delta;
            return Task.FromResult(ResourceAmount.Create(_resources.First(x => x.Id == resourceId), entity.Amount));
        }

        public Task<IReadOnlyDictionary<Guid, int>> GetResourcesAmount(Guid playerId)
        {
            var result = _resources.ToDictionary(
                resource => resource.Id,
                resource => GetResourceByPlayerInternal(resource.Id, playerId).Amount);
            return Task.FromResult<IReadOnlyDictionary<Guid, int>>(result);
        }

        public async Task<bool> PayIfEnough(Price price, Guid playerId)
        {
            foreach (var x in price.PerResourcePrice)
            {
                var resource = GetResourceByPlayerInternal(x.Key, playerId);
                if (resource.Amount < x.Value)
                    return false;
            }

            await Pay(price, playerId);
            return true;
        }

        public Task<bool> IsEnoughToPay(Price price, Guid playerId)
        {
            var result = price.PerResourcePrice.All(x =>
            {
                var resource = GetResourceByPlayerInternal(x.Key, playerId);
                return resource.Amount >= x.Value;
            });
            return Task.FromResult(result);
        }

        public Task Pay(Price price, Guid playerId)
        {
            price.PerResourcePrice.ForEach(x =>
            {
                var resource = GetResourceByPlayerInternal(x.Key, playerId);
                resource.Amount -= x.Value;
            });
            return Task.CompletedTask;
        }

        public Task GiveResources(ResourceAmount[] resourceAmounts, Guid playerId)
        {
            var price = resourceAmounts.ToPrice(true);
            return Pay(price, playerId);
        }

        private MutableResourceByPlayerEntity GetResourceByPlayerInternal(Guid resourceId, Guid playerId)
        {
            var entity = _resourceByPlayers.FirstOrDefault(x => x.PlayerId == playerId && x.ResourceId == resourceId);
            if (entity == null)
            {
                entity = new MutableResourceByPlayerEntity
                {
                    ResourceId = resourceId,
                    PlayerId = playerId,
                    Amount = 0,
                };
                _resourceByPlayers.Add(entity);
            }
            return entity;
        }
    }
}
