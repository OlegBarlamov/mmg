using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Services
{
    [UsedImplicitly]
    internal class AppDomainService : IAppDomainService
    {
        [NotNull] public IEnumerable<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAllTypes();
        }
    }
}
