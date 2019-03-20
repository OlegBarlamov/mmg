using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ConsoleWindow.Models
{
    internal class ConsoleSourceCollection : IDisposable
    {
        public ReadOnlyObservableCollection<ConsoleSource> Items { get; }

        private const string AllSourceName = "!All";
        private const string ErrorSourceName = "!Errors";
        private const string WarningsSourceName = "!Warnings";

        private readonly ObservableCollection<ConsoleSource> _items = new ObservableCollection<ConsoleSource>();

        private readonly IReadOnlyList<ConsoleSource> _internalSources;

        public ConsoleSourceCollection()
        {
            Items = new ReadOnlyObservableCollection<ConsoleSource>(_items);

            _internalSources = CreateInternalSources();
            foreach (var internalSource in _internalSources)
            {
                _items.Add(internalSource);
            }
        }

        public ConsoleSource CreateSource(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

            var newSource = new ConsoleSource(name, _internalSources);

            _items.Add(newSource);
            return newSource;
        }

        public void WriteMessage(string message, LogLevel logLevel = LogLevel.Information, string sourceName = null)
        {
            var targetSourceName = string.IsNullOrWhiteSpace(sourceName) ? AllSourceName : sourceName;

            var targetSource = Items.FirstOrDefault(source => source.Name == targetSourceName)
                               ?? CreateSource(targetSourceName);

            targetSource.Write(message, logLevel);
        }

        public void Dispose()
        {
            _items.Clear();
        }

        private IReadOnlyList<ConsoleSource> CreateInternalSources()
        {
            var allSourcesSource = new ConsoleSource(AllSourceName);
            var errorsSourcesSource = new FilteredByLevelConsoleSource(ErrorSourceName, level => level == LogLevel.Error || level == LogLevel.Critical);
            var warningSourcesSource = new FilteredByLevelConsoleSource(WarningsSourceName, level => level == LogLevel.Warning);

            return new[] {allSourcesSource, errorsSourcesSource, warningSourcesSource};
        }
    }
}