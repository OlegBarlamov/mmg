using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ConsoleWindow.Common;
using ConsoleWindow.Models;
using JetBrains.Annotations;

namespace ConsoleWindow.ViewModels
{
    internal class ConsoleControlViewModel : NotifyObject, IDisposable
    {
        public event EventHandler<string> UserCommand;

        public ConsoleContentViewModel SelectedConsoleContent
        {
            get => _selectedConsoleContent;
            set => SetProperty(ref _selectedConsoleContent, value);
        }

        public string CommandText
        {
            get => _commandText;
            set => SetProperty(ref _commandText, value);
        }

        public ICommand SubmitCommand { get; }

        public ReadOnlyObservableCollection<ConsoleContentViewModel> Contents { get; }

        public ReadOnlyObservableCollection<CommandDescription> Commands { get; }

        public IValueConverter CommandConverter { get; }

        private ConsoleSourceCollection ConsoleSourceCollection { get; }
        private Dispatcher Dispatcher { get; }
        private IConsolePalette ConsolePalette { get; }

        private readonly ObservableCollection<ConsoleContentViewModel> _contents = new ObservableCollection<ConsoleContentViewModel>();

        private ConsoleContentViewModel _selectedConsoleContent;
        private string _commandText;

        public ConsoleControlViewModel([NotNull] ConsoleSourceCollection consoleSourceCollection, [NotNull] Dispatcher dispatcher, [NotNull] IConsolePalette consolePalette,
            [NotNull] ReadOnlyObservableCollection<CommandDescription> commands)
        {
            Contents = new ReadOnlyObservableCollection<ConsoleContentViewModel>(_contents);
            ConsoleSourceCollection = consoleSourceCollection ?? throw new ArgumentNullException(nameof(consoleSourceCollection));
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            ConsolePalette = consolePalette ?? throw new ArgumentNullException(nameof(consolePalette));
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));

            AddNewSourcesInternal(ConsoleSourceCollection.Items);

            ((INotifyCollectionChanged) ConsoleSourceCollection.Items).CollectionChanged += SourcesOnCollectionChanged;

            SelectedConsoleContent = Contents.FirstOrDefault();

            SubmitCommand = new CommandUI(SubmitCommandAction);
            CommandConverter = new CommandDescriptionToTextConverter();
        }

        public void Dispose()
        {
            ((INotifyCollectionChanged) ConsoleSourceCollection.Items).CollectionChanged -= SourcesOnCollectionChanged;

            foreach (var consoleContentViewModel in _contents)
            {
                consoleContentViewModel.Dispose();
            }

            _contents.Clear();
        }

        public void ClearSelected()
        {
            SelectedConsoleContent?.Clear();
        }

        public void ClearAll()
        {
            foreach (var consoleContentViewModel in Contents)
            {
                consoleContentViewModel.Clear();
            }
        }

        private void SubmitCommandAction(object parameter)
        {
            CommandText = string.Empty;

            var commandText = parameter as string;
            if (string.IsNullOrWhiteSpace(commandText))
                return;

            UserCommand?.Invoke(this, commandText);
        }

        private void SourcesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddNewSources(e.NewItems.Cast<ConsoleSource>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddNewSourcesInternal(IEnumerable<ConsoleSource> newSources)
        {
            foreach (var consoleSource in newSources)
            {
                AddSource(consoleSource);
            }
        }
        
        private void AddNewSources(IEnumerable<ConsoleSource> newSources)
        {
            if (!Dispatcher.CheckAccess())
            {
                //this sync execution
                Dispatcher.Invoke(DispatcherPriority.Background, (Action) (() => { AddNewSourcesInternal(newSources); }));
            }
            else
            {
                AddNewSourcesInternal(newSources);
            }
        }

        private void AddSource(ConsoleSource source)
        {
            var viewModel = new ConsoleContentViewModel(source, ConsolePalette, Dispatcher);
            _contents.Add(viewModel);
        }

        private class CommandDescriptionToTextConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (!(value is CommandDescription command))
                    return value?.ToString();

                return command.CommandName;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}