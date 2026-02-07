using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.EffectTypes
{
    public interface IEffectTypesService
    {
        Task<IEffectTypeObject> GetById(Guid id);
        Task<IEffectTypeObject> GetByKey(string key);
        Task<IEffectTypeObject[]> GetAll();
    }
}
