using System.Collections.Generic;
using System.Linq;
using Epic.Core.Objects.Battle;

namespace Epic.Server.Resources
{
    public class BattleResource
    {
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyCollection<BattleUnitResource> Units { get; }
        public TurnInfoResource TurnInfo { get; }
        
        public BattleResource(IBattleObject battleObject)
        {
            Width = battleObject.Width;
            Height = battleObject.Height;
            Units = battleObject.Units.Select(x => new BattleUnitResource(x)).ToList();
            TurnInfo = new TurnInfoResource(battleObject.TurnIndex, battleObject.TurnPlayerIndex);
        }
    }
}