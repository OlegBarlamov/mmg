namespace Epic.Data.UnitTypes
{
    public interface IUnitProps
    {
        int Speed { get; }
        int AttackMaxRange { get; }
        int AttackMinRange { get; }
        int Damage { get; }
        int Health { get; }
    }
}