using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.Battles;

namespace Epic.Server.Resources
{
    public class BattleUnitResource
    {
        public Guid Id { get; }
        
        public HexoPoint Position { get; }
        
        public string Name { get; }

        public string Player { get; }
        
        public BattleUnitPropsResource Props { get; }
        
        public BattleUnitPropsResource CurrentProps { get; }
        
        public int Count { get; }
        public bool IsAlive { get; }

        public IReadOnlyList<BattleUnitBuffResource> Buffs { get; }
        
        public BattleUnitResource(IBattleUnitObject battleUnitObject)
        {
            Id = battleUnitObject.Id;
            Name = battleUnitObject.GlobalUnit.UnitType.Name;
            Position = new HexoPoint
            {
                C = battleUnitObject.Column,
                R = battleUnitObject.Row,
            };
            Player = ((InBattlePlayerNumber)battleUnitObject.PlayerIndex).ToString();
            Count = battleUnitObject.GlobalUnit.Count;
            IsAlive = battleUnitObject.GlobalUnit.IsAlive;
            Props = new BattleUnitPropsResource(battleUnitObject, false);
            CurrentProps = new BattleUnitPropsResource(battleUnitObject, true);
            Buffs = (battleUnitObject.Buffs ?? Array.Empty<IBuffObject>())
                .Select(x => new BattleUnitBuffResource(x))
                .ToList();
        }
    }
}