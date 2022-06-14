using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Services
{
    public interface IAppDomainService
    {
        [NotNull] IEnumerable<Type> GetAllTypes();

        [CanBeNull] Type FindTypeFromFullName(string typeName);

        [CanBeNull] Type FindTypeFromShortName(string name);
    }
}
