using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK
{
    internal class AppConfiguratorWithApplication<TApp> : AppConfigurator where TApp : IApplication
    {
        public AppConfiguratorWithApplication([NotNull] IPipelineProcessor pipelineProcessor)
            : base(pipelineProcessor)
        {
        }

        public override void Run()
        {
            TApp app;
            
            try
            {
                app = CreateApp();
                base.Run();
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
}