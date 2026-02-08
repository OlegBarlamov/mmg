namespace Epic.Data.Effect
{
    public class EffectEffectiveValues : IEffectProperties
    {
        public int TakesDamageMin { get; set; }
        public int TakesDamageMax { get; set; }
        public int Heals { get; set; }
        public int HealsPercentage { get; set; }
        public bool HealCanResurrect { get; set; }
        
        public static EffectEffectiveValues From(IEffectProperties source)
        {
            if (source == null)
                return new EffectEffectiveValues();
            return new EffectEffectiveValues
            {
                TakesDamageMin = source.TakesDamageMin,
                TakesDamageMax = source.TakesDamageMax,
                Heals = source.Heals,
                HealsPercentage = source.HealsPercentage,
                HealCanResurrect = source.HealCanResurrect,
            };
        }
    }
}
