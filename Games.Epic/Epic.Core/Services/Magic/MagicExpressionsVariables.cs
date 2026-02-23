using Epic.Core.Services.Buffs;
using Epic.Core.Services.Effects;
using Epic.Core.Utils;

namespace Epic.Core.Services.Magic
{
    public class MagicExpressionsVariables : ExpressionVariables
    {
        public static MagicExpressionsVariables Empty() => new MagicExpressionsVariables();
        
        public BuffExpressionsVariables ToBuffExpressionsVariables() =>
            BuffExpressionsVariables.FromVariables(this);
        
        public EffectExpressionsVariables ToEffectExpressionsVariables() =>
            EffectExpressionsVariables.FromVariables(this);
    }
}
