using System;

namespace Epic.Core.ServerMessages
{
    public class PlayerMagicCommandFromServer : PlayerCommandFromServer
    {
        public Guid MagicTypeId { get; }

        public PlayerMagicCommandFromServer(int turnNumber, InBattlePlayerNumber player, Guid magicTypeId)
            : base(turnNumber, player)
        {
            MagicTypeId = magicTypeId;
        }

        public override string Command => "PLAYER_MAGIC";
    }
}
