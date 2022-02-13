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

        public object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public object ResolveWithParameters(Type type, object[] parameters)
        {
            return Container.Resolve(type, parameters.Select(TypedParameter.From));
        }

        public IReadOnlyList<object> ResolveMultiple(Type type)
        {
            var listOf = typeof(IEnumerable<>);
            var listOfType = listOf.MakeGenericType(type);
            
            var resultArray = (Array) Container.Resolve(listOfType);
            return resultArray.Cast<object>().ToArray();
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