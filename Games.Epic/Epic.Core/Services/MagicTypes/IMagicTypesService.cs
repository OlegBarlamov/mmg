using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.MagicTypes
{
    public interface IMagicTypesService
    {
        Task<IMagicTypeObject> GetById(Guid id);
        Task<IMagicTypeObject> GetByKey(string key);
        Task<IMagicTypeObject[]> GetAll();
    }
}
