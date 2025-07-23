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
        
        public PlayerResource(IPlayerObject playerObject, bool isDefeated, Guid armyContainerId)
        {
            Id = playerObject.Id;
            UserId = playerObject.UserId;
            Day = playerObject.Day;
            Name = playerObject.Name;
            IsDefeated = isDefeated;
            BattlesGenerationInProgress = playerObject.GenerationInProgress;
            ArmyContainerId = armyContainerId.ToString();
            SupplyContainerId = playerObject.Supply.Id.ToString();
        }
    }
}
