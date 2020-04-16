using System;
using System.Collections.Generic;
using Console.Core.Implementations.ExternalProcess.ProcessMessages;
using Console.Core.Models;

namespace Console.Core.Implementations.ExternalProcess
{
    public class ExternalProcessConsoleController : IConsoleController
    {
        public bool IsShowed { get; private set; }
        private IConsoleMessagesProvider MessagesProvider { get; }
        private IConsoleCommandExecutor CommandExecutor { get; }

        private readonly ConsoleProcessWrapper _processWrapper;
        
        public ExternalProcessConsoleController(IConsoleMessagesProvider messagesProvider, IConsoleCommandExecutor commandExecutor, string executiveFilePath)
        {
            MessagesProvider = messagesProvider ?? throw new ArgumentNullException(nameof(messagesProvider));
            CommandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
            
            var binaryFormatter = new BinaryConsoleProcessMessageFormatter();
            var safeFormatterWrapper = new MessageFormatterSafeWrapper(binaryFormatter, OnSerializeError, OnDeserializeError);
            _processWrapper = new ConsoleProcessWrapper(executiveFilePath, safeFormatterWrapper);
            _processWrapper.NewUserCommand += ProcessWrapperOnNewUserCommand;
            
            MessagesProvider.NewMessages += MessagesesProviderOnNewMessages;
        }

        public void Dispose()
        {
            MessagesProvider.NewMessages -= MessagesesProviderOnNewMessages;
            _processWrapper.NewUserCommand -= ProcessWrapperOnNewUserCommand;
            _processWrapper.Stop();
            IsShowed = false;
            
            _processWrapper.Dispose();
        }
        
        public void Show()
        {
            if (!_processWrapper.IsRunning)
            {
                _processWrapper.Run();
            }
            
            _processWrapper.SendShowCommand();
            IsShowed = true;
        }

        public void Hide()
        {
            if (_processWrapper.IsRunning)
            {
                _processWrapper.SendHideCommand();
            }

            IsShowed = false;
        }

        public void ClearCurrent()
        {
            if (_processWrapper.IsRunning)
            {
                _processWrapper.SendClearCurrentCommand();
            }
        }

        public void ClearAll()
        {
            if (_processWrapper.IsRunning)
            {
                _processWrapper.SendClearAllCommand();
            }
        }
        
        private async void MessagesesProviderOnNewMessages()
        {
            if (!_processWrapper.IsRunning)
            {
                _processWrapper.Run();
                await _processWrapper.WaitWhileStarted().ConfigureAwait(false);
            }

            var newMessages = new List<IConsoleMessage>();
            while (!MessagesProvider.IsQueueEmpty)
            {
                var message = MessagesProvider.Pop();
                newMessages.Add(message);
            }
            
            _processWrapper.NewMessages(newMessages);
        }
        
        private async void ProcessWrapperOnNewUserCommand(string command)
        {
            try
            {
                await CommandExecutor.ExecuteAsync(command);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Unhandled exception while console command executing: " + e);
            }
        }
        
        private void OnSerializeError(Exception error, IConsoleProcessMessage message)
        {
            System.Console.WriteLine($"Console process message {message.ToString()} serialization error: " + error);
        }
        
        private void OnDeserializeError(Exception error)
        {
            System.Console.WriteLine("Console process message deserialization error: " + error);
        }
    }
}