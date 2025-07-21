using System;
using Epic.Core.Services.Players;

namespace Epic.Server.Resources
{
    public class PlayerResource
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public int Day { get; }
        public string Name { get; }
        public bool IsDefeated { get; }
        public bool BattlesGenerationInProgress { get; }
        public string ArmyContainerId { get; }
        public string SupplyContainerId { get; }
        
        public PlayerResource(IPlayerObject playerObject)
        {
            Id = playerObject.Id;
            UserId = playerObject.UserId;
            Day = playerObject.Day;
            Name = playerObject.Name;
            IsDefeated = playerObject.IsDefeated;
            BattlesGenerationInProgress = playerObject.GenerationInProgress;
            ArmyContainerId = playerObject.Army.Id.ToString();
            SupplyContainerId = playerObject.Supply.Id.ToString();
        }
    }
}
