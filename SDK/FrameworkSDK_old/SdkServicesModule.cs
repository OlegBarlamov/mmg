using System;
using Autofac;
using FrameworkSDK.Implementations;
using FrameworkSDK.Services;
using JetBrains.Annotations;

namespace FrameworkSDK
{
	internal class SdkServicesModule : IServicesModule
	{
		[NotNull] private ApplicationBase Application { get; }

		public SdkServicesModule([NotNull] ApplicationBase application)
		{
			Application = application ?? throw new ArgumentNullException(nameof(application));
		}

		public void RegisterServices(ContainerBuilder container)
		{
			container.RegisterType<DefaultViewFactory>().As<IViewFactory>().SingleInstance();
			container.RegisterInstance(new GraphicsService(Application)).As<IGraphicsService>().SingleInstance();
		}
	}
}
