using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.BuffTypes
{
    public interface IBuffTypesService
    {
        Task<IBuffTypeObject> GetById(Guid id);
        Task<IBuffTypeObject> GetByKey(string key);
        Task<IBuffTypeObject[]> GetAll();
    }
}

