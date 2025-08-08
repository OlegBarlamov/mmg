namespace Epic.Core.Services.Units
{
    public static class UnitsExtensions
    {
        public static bool IsSameType(this IGlobalUnitObject unit1, IGlobalUnitObject unit2)
        {
            return unit1.UnitType.Id == unit2.UnitType.Id;
        }
    }
}