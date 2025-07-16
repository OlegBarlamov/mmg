using System;
using Epic.Core;
using Epic.Core.Objects.BattleUnit;

namespace Epic.Server.Resources
{
    public class BattleUnitResource
    {
        public Guid Id { get; }
        
        public HexoPoint Position { get; }

        public string Player { get; }
        
        public BattleUnitPropsResource Props { get; }
        
        public BattleUnitPropsResource CurrentProps { get; }
        
        public int Count { get; }
        public Guid UserId { get; }
        public bool IsAlive { get; }
        
        public BattleUnitResource(IBattleUnitObject battleUnitObject)
        {
            Id = battleUnitObject.Id;
            Position = new HexoPoint
            {
                C = battleUnitObject.Column,
                R = battleUnitObject.Row,
            };
            Player = ((InBattlePlayerNumber)battleUnitObject.PlayerIndex).ToString();
            Count = battleUnitObject.UserUnit.Count;
            UserId = battleUnitObject.UserUnit.UserId;
            IsAlive = battleUnitObject.UserUnit.IsAlive;
            Props = new BattleUnitPropsResource(battleUnitObject, false);
            CurrentProps = new BattleUnitPropsResource(battleUnitObject, true);
        }
    }
}