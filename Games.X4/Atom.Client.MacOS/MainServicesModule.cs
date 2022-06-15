using System;
using System.Linq;
using System.Reflection;
using Atom.Client.MacOS.Resources;
using Atom.Client.MacOS.Services.Implementations;
using Console.Core.Commands;
using Console.FrameworkAdapter;
using Console.FrameworkAdapter.Commands;
using Console.InGame.Commands;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Implementations;
using JetBrains.Annotations;
using NetExtensions;

namespace Atom.Client.MacOS
{
    [UsedImplicitly]
    public class MainServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<MainSceneDataModel, MainSceneDataModel>();
            serviceRegistrator.RegisterInstance(new ScenesResolverHolder());
            serviceRegistrator.RegisterFactory<IScenesResolver>((locator, type) => locator.Resolve<ScenesResolverHolder>().ScenesResolver);
            serviceRegistrator.RegisterType<IExecutableCommandsCollection, ExecutableCommandsCollection>();
            
            serviceRegistrator.RegisterInstance<IMainUpdatesTasksProcessor>(new FixedVelocityTasksProcessor(5));
            serviceRegistrator.RegisterInstance<IBackgroundTasksProcessor>(new ThreadPoolTasksProcessor());
            
            serviceRegistrator.RegisterType<MainResourcePackage, MainResourcePackage>();
            
            RegisterConsoleCommands(serviceRegistrator);
        }

        private void RegisterConsoleCommands(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IExecutableConsoleCommand, AddLogLevelFilterCommand>();
            serviceRegistrator.RegisterType<IExecutableConsoleCommand, AddSourceFilterCommand>();
            serviceRegistrator.RegisterType<IExecutableConsoleCommand, ClearFilterCommand>();
            serviceRegistrator.RegisterType<IExecutableConsoleCommand, ShowEnumValuesConsoleCommand>();
            
            var consoleCommandTypes = AppDomain.CurrentDomain.GetAllTypes()
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => type.GetCustomAttribute<RegisterConsoleCommandAttribute>(false) != null);

            foreach (var consoleCommandType in consoleCommandTypes)
            {
                serviceRegistrator.RegisterType(typeof(IExecutableConsoleCommand), consoleCommandType);
            }
        }
    }
}