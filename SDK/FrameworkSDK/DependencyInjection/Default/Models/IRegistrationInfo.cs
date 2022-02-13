using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
    public interface IRegistrationInfo
    {
        [NotNull] Type Type { get; }

        ResolveType ResolveType { get; }

        string ToString();
        
        object GetOrSet(Type type, IServiceLocator serviceLocator, object[] parameters);

        IEnumerable<IDisposable> GetDisposableCashedObjects();
    }
}