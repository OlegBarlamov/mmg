using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Objects;
using Epic.Core.Services.Battles;
using Epic.Data.Heroes;

namespace Epic.Server.Resources
{
    public class BattleResource
    {
        public Guid Id { get; }
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyCollection<BattleUnitResource> Units { get; }
        public TurnInfoResource TurnInfo { get; }
        public IReadOnlyList<PlayerInBattleInfoResource> Players { get; }
        public IReadOnlyList<BattleObstacleResource> Obstacles { get; }
        
        public BattleResource(IBattleObject battleObject, IReadOnlyDictionary<Guid, IHeroStats> playerHeroStats)
        {
            Id = battleObject.Id;
            Width = battleObject.Width;
            Height = battleObject.Height;
            Units = battleObject.Units.Select(x => new BattleUnitResource(x)).ToList();
            TurnInfo = new TurnInfoResource(battleObject.TurnNumber, battleObject.TurnPlayerIndex, battleObject.RoundNumber);
            Players = battleObject.PlayerInfos
                .Select(x => 
                {
                    var heroStats = playerHeroStats.TryGetValue(x.PlayerId, out var stats) 
                        ? stats 
                        : DefaultHeroStats.Instance;
                    return new PlayerInBattleInfoResource(x, battleObject.FindPlayerNumber(x).Value, heroStats);
                })
                .ToList();
            Obstacles = battleObject.Obstacles.Select(x => new BattleObstacleResource(x)).ToList();
        }
    }
}