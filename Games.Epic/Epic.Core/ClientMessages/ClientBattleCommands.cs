using System;
using System.Collections.Generic;

namespace Epic.Core.ClientMessages
{
    public static class ClientBattleCommands
    {
        public const string CLIENT_CONNECTED = "CLIENT_CONNECTED";
        public const string UNIT_ATTACK = "UNIT_ATTACK";
        public const string UNIT_MOVE = "UNIT_MOVE";
        public const string UNIT_WAIT = "UNIT_WAIT";
        public const string UNIT_PASS = "UNIT_PASS";
        
        public static IReadOnlyDictionary<string, Type> CommandTypes { get; } = new Dictionary<string, Type>
        {
            { CLIENT_CONNECTED, typeof(ClientConnectedBattleMessage) },
            { UNIT_MOVE, typeof(UnitMoveClientBattleMessage) },
            { UNIT_ATTACK, typeof(UnitAttackClientBattleMessage) },
            { UNIT_WAIT, typeof(UnitWaitClientBattleMessage) },
            { UNIT_PASS, typeof(UnitPassClientBattleMessage) },
        };
    }
}