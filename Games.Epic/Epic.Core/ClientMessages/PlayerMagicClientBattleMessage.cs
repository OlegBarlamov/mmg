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
        /// <summary>Required for Enemy/Ally/All* single-target: the chosen unit. Not used for Location.</summary>
        public Guid? TargetUnitId { get; set; }
        /// <summary>Required for Location cast target type: row of the target cell.</summary>
        public int? TargetRow { get; set; }
        /// <summary>Required for Location cast target type: column of the target cell.</summary>
        public int? TargetColumn { get; set; }
    }
}
