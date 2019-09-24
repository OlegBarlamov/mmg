using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
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
            var app = CreateApp();

            base.Run();

            app.Run();
        }

        private static TApp CreateApp()
        {
            if (AppContext.ServiceLocator != null)
                return AppContext.ServiceLocator.Resolve<TApp>();

            return Activator.CreateInstance<TApp>();
        }
    }
}