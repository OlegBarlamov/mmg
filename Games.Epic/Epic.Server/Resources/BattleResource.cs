using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Battles;

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
        
        public BattleResource(IBattleObject battleObject)
        {
            Id = battleObject.Id;
            Width = battleObject.Width;
            Height = battleObject.Height;
            Units = battleObject.Units.Select(x => new BattleUnitResource(x)).ToList();
            TurnInfo = new TurnInfoResource(battleObject.TurnNumber, battleObject.TurnPlayerIndex, battleObject.RoundNumber);
            Players = battleObject.PlayerInfos
                .Select(x => new PlayerInBattleInfoResource(x, battleObject.FindPlayerNumber(x).Value))
                .ToList();
        }
    }
}