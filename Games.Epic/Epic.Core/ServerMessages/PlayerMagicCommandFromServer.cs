using System;

namespace Epic.Core.ServerMessages
{
    public class PlayerMagicCommandFromServer : PlayerCommandFromServer
    {
        public Guid MagicTypeId { get; }
        public string MagicName { get; }
        public int? CurrentMana { get; }

        public PlayerMagicCommandFromServer(int turnNumber, InBattlePlayerNumber player, Guid magicTypeId, string magicName = null, int? currentMana = null)
            : base(turnNumber, player)
        {
            MagicTypeId = magicTypeId;
            MagicName = magicName;
            CurrentMana = currentMana;
        }

        public override string Command => "PLAYER_MAGIC";
    }
}
