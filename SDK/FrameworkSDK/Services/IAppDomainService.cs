using System;
using System.Collections.Generic;

namespace FrameworkSDK.Services
{
    public interface IAppDomainService
    {
        IEnumerable<Type> GetAllTypes();
    }
}
