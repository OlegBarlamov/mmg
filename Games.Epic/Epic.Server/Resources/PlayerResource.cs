using System;
using Epic.Core.Objects;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Data.Heroes;

namespace Epic.Server.Resources
{
    public class PlayerResource
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public int Day { get; }
        public int Stage { get; }
        public string Name { get; }
        public bool IsDefeated { get; }
        public bool BattlesGenerationInProgress { get; }
        public string ArmyContainerId { get; }
        public string SupplyContainerId { get; }
        public int Experience { get; }
        public int Level { get; }
        public IHeroStats Stats { get; }

        public PlayerResource(IPlayerObject playerObject, bool isDefeated, Guid armyContainerId)
        {
            Id = playerObject.Id;
            UserId = playerObject.UserId;
            Day = playerObject.Day;
            Stage = playerObject.Stage;
            Name = playerObject.Name;
            IsDefeated = isDefeated;
            BattlesGenerationInProgress = playerObject.GenerationInProgress;
            ArmyContainerId = armyContainerId.ToString();
            SupplyContainerId = playerObject.Supply.Id.ToString();
            Experience = playerObject.ActiveHero.Experience;
            Level = playerObject.ActiveHero.Level;
            Stats = playerObject.ActiveHero?.GetCumulativeHeroStats() ?? DefaultHeroStats.Instance;
        }
    }
}
