namespace Epic.Data.Effect
{
    public interface IEffectProperties
    {
        int TakesDamageMin { get; }
        int TakesDamageMax { get; }
    
        int Heals { get; }
        int HealsPercentage { get; }
        bool HealCanResurrect { get; }
    }
}