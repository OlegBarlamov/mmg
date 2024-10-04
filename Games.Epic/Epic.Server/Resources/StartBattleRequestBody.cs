using System;

namespace Epic.Server.Resources
{
    public class StartBattleRequestBody
    {
        public string UserId { get; set; }
        public string BattleDefinitionId { get; set; }
    }
}