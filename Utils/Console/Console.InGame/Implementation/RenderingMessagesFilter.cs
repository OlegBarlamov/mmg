using System;
using System.Collections.Generic;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.InGame.Implementation
{
    internal interface IRenderingMessagesFilter
    {
        bool Validate(IRenderingMessage renderingMessage);
    }
    
    internal class RenderingMessagesFilter : IRenderingMessagesFilter
    {
        private readonly List<string> _filteredSources = new List<string>();
        private readonly object _filteredSourcesLocker = new object();
        private readonly List<ConsoleLogLevel> _filteredLogLevels = new List<ConsoleLogLevel>();
        private readonly object _filteredLogLevelsLocker = new object();

        public string GetFilter()
        {
            var sourcesFilterName = "*";
            var logLevelsFilterName = "*";

            // ReSharper disable once InconsistentlySynchronizedField
            if (_filteredSources.Count != 0)
            {
                lock (_filteredSourcesLocker)
                {
                    sourcesFilterName = $"[{string.Join(",", _filteredSources)}]";
                }
            }

            // ReSharper disable once InconsistentlySynchronizedField
            if (_filteredLogLevels.Count != 0)
            {
                lock (_filteredLogLevelsLocker)
                {
                    logLevelsFilterName = $"[{string.Join(",", _filteredLogLevels)}]";
                }
            }

            return $"S:{sourcesFilterName} & L: {logLevelsFilterName}";
        }
        
        public void Add(ConsoleLogLevel logLevel)
        {
            lock (_filteredLogLevelsLocker)
            {
                _filteredLogLevels.Add(logLevel);
            }
        }

        public void Add([NotNull] string source)
        {
            if (string.IsNullOrWhiteSpace(source)) throw new ArgumentException(nameof(source));

            lock (_filteredSourcesLocker)
            {
                _filteredSources.Add(source);
            }
        }

        public void ClearFilters()
        {
            lock (_filteredSourcesLocker)
            {
                _filteredSources.Clear();
            }
            
            lock (_filteredLogLevelsLocker)
            {
                _filteredLogLevels.Clear();
            }
        }
        
        public bool Validate(IRenderingMessage renderingMessage)
        {
            var result = true;
            
            // ReSharper disable once InconsistentlySynchronizedField
            if (_filteredSources.Count != 0)
            {
                lock (_filteredSourcesLocker)
                {
                    result = _filteredSources.Contains(renderingMessage.Source);
                }
            }

            // ReSharper disable once InconsistentlySynchronizedField
            if (_filteredLogLevels.Count != 0)
            {
                lock (_filteredLogLevelsLocker)
                {
                    result = result && _filteredLogLevels.Contains(renderingMessage.LogLevel);
                }
            }

            return result;
        }
    }
}