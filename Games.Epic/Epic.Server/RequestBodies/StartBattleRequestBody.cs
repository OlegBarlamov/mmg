using System;

namespace Epic.Server.RequestBodies
{
    public class StartBattleRequestBody
    {
        public Guid BattleDefinitionId { get; set; }
    }

    public class StartBattleWithPlayerRequestBody
    {
        public string PlayerName { get; set; }
    }
}