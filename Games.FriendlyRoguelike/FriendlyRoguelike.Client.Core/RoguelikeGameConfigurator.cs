using System;
using Console.Core;
using FrameworkSDK.Constructing;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FriendlyRoguelike.Core
{
    public class RoguelikeGameConfigurator : IAppConfigurator
    {
        public IConsoleMessagesProvider ConsoleMessagesProvider { get; }
        Pipeline IAppConfigurator.ConfigurationPipeline => _configurator.ConfigurationPipeline;
        
        private readonly IAppConfigurator _configurator;

        public RoguelikeGameConfigurator([NotNull] IAppConfigurator configurator, [NotNull] IConsoleMessagesProvider consoleMessagesProvider)
        {
            ConsoleMessagesProvider = consoleMessagesProvider ?? throw new ArgumentNullException(nameof(consoleMessagesProvider));
            _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
        }
        
        void IDisposable.Dispose()
        {
            _configurator.Dispose();
        }
        
        public IAppRunner Configure()
        {
            return _configurator.Configure();
        }
    }
}