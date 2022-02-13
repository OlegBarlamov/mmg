using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Services
{
    public interface IAppDomainService
    {
        [NotNull] IEnumerable<Type> GetAllTypes();
    }
}
