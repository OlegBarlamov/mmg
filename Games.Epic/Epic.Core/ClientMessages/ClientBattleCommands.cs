using System;
using System.Collections.Generic;

namespace Epic.Core.ClientMessages
{
    public static class ClientBattleCommands
    {
        public const string CLIENT_CONNECTED = "CLIENT_CONNECTED";
        public const string UNIT_ATTACK = "UNIT_ATTACK";
        public const string UNIT_MOVE = "UNIT_MOVE";
        
        public static IReadOnlyDictionary<string, Type> CommandTypes { get; } = new Dictionary<string, Type>
        {
            { CLIENT_CONNECTED, typeof(ClientConnectedBattleMessage) },
            { UNIT_MOVE, typeof(UnitMoveClientBattleMessage) },
            { UNIT_ATTACK, typeof(UnitAttackClientBattleMessage) }
        };
    }
}