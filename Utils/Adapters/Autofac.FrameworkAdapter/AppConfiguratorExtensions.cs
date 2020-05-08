using System;
using FrameworkSDK.Constructing;
using JetBrains.Annotations;

namespace Autofac.FrameworkAdapter
{
    public static class AppConfiguratorExtensions
    {
        public static IAppConfigurator UseAutofac([NotNull] this IAppConfigurator appConfigurator)
        {
            if (appConfigurator == null) throw new ArgumentNullException(nameof(appConfigurator));
            return appConfigurator.SetupCustomIoc(() => new AutofacServiceContainerFactory());
        }
    }
}