using System;
using Gates.ClientCore.Logging;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace Gates.ClientCore.ExternalCommands
{
    internal class ExternalCommandsProcessor : IExternalCommandsProcessor
    {
        private IClientHost ClientHost { get; }
        private IExternalCommandParser CommandParser { get; }
        private ILogger Logger { get; }

        public ExternalCommandsProcessor(
            [NotNull] IClientHost clientHost,
            [NotNull] IExternalCommandParser commandParser,
            [NotNull] ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            ClientHost = clientHost ?? throw new ArgumentNullException(nameof(clientHost));
            CommandParser = commandParser ?? throw new ArgumentNullException(nameof(commandParser));
            Logger = loggerFactory.CreateLogger(LogCategories.ExternalCommands);
        }

        public void ProcessCommand(string commandLine)
        {
            var command = CommandParser.Parse(commandLine);
            if (command.IsEmpty)
                return;

            try
            {
                //TODO сделать нормально
                if (command.Name == "srv_connect")
                {
                    var url = command.Parameters[0];
                    ClientHost.ConnectToServer(url);
                    return;
                }
                if (command.Name == "srv_auth")
                {
                    var name = command.Parameters[0];
                    ClientHost.Authorize(name);
                    return;
                }
                if (command.Name == "rm_create")
                {
                    var name = command.Parameters[0];
                    var pass = command.Parameters[1];
                    ClientHost.CreateRoom(name, pass);
                    return;
                }
                if (command.Name == "rm_enter")
                {
                    var name = command.Parameters[0];
                    var pass = command.Parameters[1];
                    ClientHost.EnterRoom(name, pass);
                    return;
                }
                if (command.Name == "start")
                {
                    ClientHost.RunGame();
                    return;
                }

            }
            catch (Exception e)
            {
                var message = e.Message;
                Logger.Error($"Error: {message}");
            }
            

            Logger.Info($"Unknown command: '{command.Name}'");
        }
    }
}
