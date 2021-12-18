using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK
{
    internal class AppConfiguratorWithApplication<TApp>: IAppConfigurator where TApp : IApplication
    {
        private class AppRunnerWithApplication : IAppRunner
        {
            private IAppRunner AppRunner { get; }

            public AppRunnerWithApplication([NotNull] IAppRunner appRunner)
            {
                AppRunner = appRunner ?? throw new ArgumentNullException(nameof(appRunner));
            }


            public void Dispose()
            {
                AppRunner.Dispose();
            }

            public void Run()
            {
                TApp app;
            
                try
                {
                    app = CreateApp();
                    AppRunner.Run();
                }
                catch (Exception e)
                {
                    throw new AppConstructingException(Strings.Exceptions.Constructing.RunAppFailed, e, typeof(TApp).Name);
                }

                try
                {
                    app.Run();
                }
                catch (Exception e)
                {
                    throw new FrameworkException(Strings.Exceptions.FatalException, e);
                }
            }
            
            private static TApp CreateApp()
            {
                if (AppContext.ServiceLocator != null)
                    return AppContext.ServiceLocator.Resolve<TApp>();

                return Activator.CreateInstance<TApp>();
            }
        }
        [NotNull] private IAppConfigurator AppConfigurator { get; }

        Pipeline IAppConfigurator.ConfigurationPipeline => AppConfigurator.ConfigurationPipeline;

        public AppConfiguratorWithApplication([NotNull] IAppConfigurator appConfigurator)
        {
            AppConfigurator = appConfigurator ?? throw new ArgumentNullException(nameof(appConfigurator));
        }

        public void Dispose()
        {
            AppConfigurator.Dispose();
        }
        
        IAppRunner IAppConfigurator.Configure()
        {
            var runner = AppConfigurator.Configure();
            return new AppRunnerWithApplication(runner);
        }
    }
}