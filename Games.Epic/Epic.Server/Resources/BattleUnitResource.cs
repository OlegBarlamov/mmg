using System;
using Epic.Core;
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
        }
    }
}