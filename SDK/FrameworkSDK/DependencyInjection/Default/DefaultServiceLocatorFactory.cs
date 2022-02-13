using System;
using FrameworkSDK.Common;
using FrameworkSDK.DependencyInjection.Default.Misc;
using FrameworkSDK.DependencyInjection.Default.Models;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default
{
    [UsedImplicitly]
    internal class DefaultServiceLocatorFactory
    {
        public IDependencyResolver DependencyResolver { get; }
        public IConstructorFinder ConstructorFinder { get; }
        public IServicesCandidatesFinder ServicesCandidatesFinder { get; }
        public IServicesRegistrationsPriority ServicesRegistrationsPriority { get; }
        public IFrameworkLogger FrameworkLogger { get; }

        public DefaultServiceLocatorFactory(
            [NotNull] IDependencyResolver dependencyResolver,
            [NotNull] IConstructorFinder constructorFinder,
            [NotNull] IServicesCandidatesFinder servicesCandidatesFinder,
            [NotNull] IServicesRegistrationsPriority servicesRegistrationsPriority,
            [NotNull] IFrameworkLogger frameworkLogger)
        {
            DependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            ConstructorFinder = constructorFinder ?? throw new ArgumentNullException(nameof(constructorFinder));
            ServicesCandidatesFinder = servicesCandidatesFinder ?? throw new ArgumentNullException(nameof(servicesCandidatesFinder));
            ServicesRegistrationsPriority = servicesRegistrationsPriority ?? throw new ArgumentNullException(nameof(servicesRegistrationsPriority));
            FrameworkLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));
        }

        public DefaultServiceLocator Create(
            [NotNull] RegistrationsDomain domain,
            [NotNull] IDisposableExtended lifeTimeScope,
            [CanBeNull] string name = null)
        {
            var serviceLocator = new DefaultServiceLocator(
                FrameworkLogger,
                lifeTimeScope,
                domain,
                name
            );
            serviceLocator.InitializeInternalServices(
                ConstructorFinder,
                DependencyResolver,
                ServicesRegistrationsPriority,
                ServicesCandidatesFinder);
            return serviceLocator;
        }

        public static DefaultServiceLocator CreateFirstDefaultServiceLocator(
            [NotNull] IFrameworkLogger logger,
            [NotNull] RegistrationsDomain domain,
            [NotNull] IDisposableExtended lifeTimeScope,
            [CanBeNull] string name = null)
        {
            var serviceLocator = new DefaultServiceLocator(logger, lifeTimeScope, domain, name);
            serviceLocator.InitializeInternalServices(
                new DefaultConstructorFinder(),
                new DefaultDependencyResolver(),
                new DefaultServicesRegistrationsPriority(), 
                new DefaultServicesCandidatesFinder()
            );
            return serviceLocator;
        }
    }
}