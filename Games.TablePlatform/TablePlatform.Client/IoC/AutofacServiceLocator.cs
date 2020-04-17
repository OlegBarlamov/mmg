using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FrameworkSDK.IoC;
using JetBrains.Annotations;

namespace TablePlatform.Client.IoC
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
            return ((IEnumerable<object>) Container.Resolve(listOfType)).ToArray();
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