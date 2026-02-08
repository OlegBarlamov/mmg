using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.Effects
{
    public interface IEffectsService
    {
        Task<IEffectObject> Create(Guid effectTypeId, EffectExpressionsVariables variables = null);
    }
}
