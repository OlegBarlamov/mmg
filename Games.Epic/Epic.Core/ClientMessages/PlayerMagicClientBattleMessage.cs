using System;

namespace Epic.Core.ClientMessages
{
    public class PlayerMagicClientBattleMessage : IClientBattleMessage
    {
        public string CommandId { get; set; }
        public string Command { get; set; } = ClientBattleCommands.PLAYER_MAGIC;
        public InBattlePlayerNumber Player { get; set; }
        public int TurnIndex { get; set; }
        public Guid MagicTypeId { get; set; }
    }
}
