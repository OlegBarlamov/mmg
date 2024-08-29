using System;
using System.Threading.Tasks;
using Console.Core.Commands;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Server.Commands
{
    [RegisterConsoleCommand]
    public class StatusCommand : FixedTypedExecutableConsoleCommand
    {
        public override string Text { get; } = "status";
        public override string Description { get; }
        
        [NotNull] private ILogger<StatusCommand> Logger { get; }

        public StatusCommand([NotNull] ILogger<StatusCommand> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        protected override Task ExecuteAsync()
        {
            Logger.LogInformation("Ok");
            return Task.CompletedTask;
        }
    }
}