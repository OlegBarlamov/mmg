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
        
        public BattleResource(IBattleObject battleObject)
        {
            Id = battleObject.Id;
            Width = battleObject.Width;
            Height = battleObject.Height;
            Units = battleObject.Units.Select(x => new BattleUnitResource(x)).ToList();
            TurnInfo = new TurnInfoResource(battleObject.TurnNumber, battleObject.TurnPlayerIndex);
        }
    }
}