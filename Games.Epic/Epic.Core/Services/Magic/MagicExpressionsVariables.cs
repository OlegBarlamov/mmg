using Epic.Core.Services.Buffs;
using Epic.Core.Services.Effects;
using Epic.Core.Utils;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Magic
{
    public class MagicExpressionsVariables : ExpressionVariables
    {
        public static MagicExpressionsVariables Empty() => new MagicExpressionsVariables();

        /// <summary>Creates variables from hero stats for evaluating magic expressions (e.g. level, power, knowledge).</summary>
        public static MagicExpressionsVariables FromHero(IHeroStats heroStats)
        {
            if (heroStats == null)
                return Empty();
            var v = new MagicExpressionsVariables();
            v.Variables["level"] = heroStats.Level;
            v.Variables["attack"] = heroStats.Attack;
            v.Variables["defense"] = heroStats.Defense;
            v.Variables["power"] = heroStats.Power;
            v.Variables["knowledge"] = heroStats.Knowledge;
            v.Variables["currentMana"] = heroStats.CurrentMana;
            v.Variables["experience"] = heroStats.Experience;
            return v;
        }

        public BuffExpressionsVariables ToBuffExpressionsVariables() =>
            BuffExpressionsVariables.FromVariables(this);

        public EffectExpressionsVariables ToEffectExpressionsVariables() =>
            EffectExpressionsVariables.FromVariables(this);
    }
}
