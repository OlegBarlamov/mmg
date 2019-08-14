using System;
using System.Collections.Generic;
using NetExtensions;

namespace FrameworkSDK.Services
{
    internal class AppDomainService : IAppDomainService
    {
        public IEnumerable<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAllTypes();
        }
    }
}
