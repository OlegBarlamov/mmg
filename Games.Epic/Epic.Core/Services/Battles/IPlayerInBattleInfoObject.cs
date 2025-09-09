using System;
using Epic.Core.Objects;
using Epic.Data.Battles;

namespace Epic.Core.Services.Battles
{
    public interface IPlayerInBattleInfoObject : IGameObject<IPlayerToBattleEntity>
    {
        Guid Id { get; }
        Guid PlayerId { get; }
        bool RansomClaimed { get; }
    }

    public class MutablePlayerInBattleInfoObject : IPlayerInBattleInfoObject
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public bool RansomClaimed { get; set; }

        private MutablePlayerInBattleInfoObject(Guid id)
        {
            Id = id;
        }

        public static MutablePlayerInBattleInfoObject FromEntity(IPlayerToBattleEntity entity)
        {
            return new MutablePlayerInBattleInfoObject(entity.Id)
            {
                PlayerId = entity.PlayerId,
                RansomClaimed = entity.RansomClaimed
            };
        }

        public static MutablePlayerInBattleInfoObject CopyFrom(IPlayerInBattleInfoObject instance)
        {
            return new MutablePlayerInBattleInfoObject(instance.Id)
            {
                PlayerId = instance.PlayerId,
                RansomClaimed = instance.RansomClaimed
            };
        }

        public IPlayerToBattleEntity ToEntity()
        {
            return new PlayerToBattleEntity
            {
                Id = Id,
                PlayerId = PlayerId,
                RansomClaimed = RansomClaimed,
            };
        }
    }
}