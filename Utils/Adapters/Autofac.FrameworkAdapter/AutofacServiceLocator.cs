using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace Autofac.FrameworkAdapter
{
    public class AutofacServiceLocator : IServiceLocator, IDisposable
    {
        public IContainer Container { get; }

        public AutofacServiceLocator([NotNull] IContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }
        
        public object Resolve(Type type, object[] additionalParameters = null)
        {
            return Container.Resolve(type, additionalParameters.Select(TypedParameter.From));
        }

        Array IServiceLocator.ResolveMultiple(Type type)
        {
            var listOf = typeof(IEnumerable<>);
            var listOfType = listOf.MakeGenericType(type);
            
            return (Array) Container.Resolve(listOfType);
        }
        
        public object CreateInstance(Type type, object[] parameters = null)
        {
            throw new NotImplementedException();
        }

        public bool IsServiceRegistered(Type type)
        {
            return Container.IsRegistered(type);
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}