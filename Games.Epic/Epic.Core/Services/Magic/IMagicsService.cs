using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.Magic
{
    public interface IMagicsService
    {
        Task<IMagicObject> Create(Guid magicTypeId, MagicExpressionsVariables variables = null);
    }
}
