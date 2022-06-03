using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    [UsedImplicitly]
    internal class DefaultDebugInfoService : IDebugInfoService
    {
        #region Measures

        private readonly ConcurrentDictionary<string, DateTime> _startMeasuringTimes = new ConcurrentDictionary<string, DateTime>();
        private readonly ConcurrentDictionary<string, TimeSpan> _resultMeasuringTimes = new ConcurrentDictionary<string, TimeSpan>();
        
        public void StartMeasure(string key)
        {
            _startMeasuringTimes.AddOrUpdate(key, Now(), (s, time) => Now());
        }

        public void StopMeasure(string key)
        {
            _resultMeasuringTimes.AddOrUpdate(key, GetMeasuredTime(key), (s, span) => GetMeasuredTime(key));
        }

        public TimeSpan GetMeasure(string key)
        {
            if (!_resultMeasuringTimes.ContainsKey(key))
                return TimeSpan.Zero;
            return _resultMeasuringTimes[key];
        }

        public IReadOnlyDictionary<string, TimeSpan> GetAllMeasures()
        {
            return _resultMeasuringTimes;
        }

        #endregion

        #region Timers

        private readonly ConcurrentDictionary<string, DateTime> _startTimers = new ConcurrentDictionary<string, DateTime>();
        
        public void StartTimer(string key)
        {
            _startTimers.TryAdd(key, Now());
        }

        public TimeSpan GetTimer(string key)
        {
            var endTime = Now();
            if (_startTimers.TryGetValue(key, out var startTime))
                return endTime - startTime;
            
            return TimeSpan.Zero;
        }

        public IReadOnlyDictionary<string, DateTime> GetAllTimers()
        {
            return _startTimers;
        }

        private TimeSpan GetMeasuredTime(string key)
        {
            var endTime = Now();
            if (_startMeasuringTimes.TryGetValue(key, out var startTime))
                return endTime - startTime;
            
            return TimeSpan.Zero;
        }

        #endregion
        
        #region Counters

        private readonly ConcurrentDictionary<string, int> _counters = new ConcurrentDictionary<string, int>();
        
        public void IncrementCounter(string key)
        {
            _counters.AddOrUpdate(key, 1, (s, i) => i + 1);
        }

        public void DecrementCounter(string key)
        {
            _counters.AddOrUpdate(key, -1, (s, i) => i - 1);
        }

        public void SetCounter(string key, int value)
        {
            _counters.AddOrUpdate(key, value, (s, i) => value);
        }

        public int GetCounter(string key)
        {
            if (_counters.TryGetValue(key, out var value))
                return value;
            
            return 0;
        }

        public IReadOnlyDictionary<string, int> GetAllCounters()
        {
            return _counters;
        }

        #endregion

        #region Labels

        private readonly ConcurrentDictionary<string, string> _labelsMap = new ConcurrentDictionary<string, string>();
        
        public void SetLabel(string key, string label)
        {
            _labelsMap.AddOrUpdate(key, label, (s, s1) => label);
        }

        public IReadOnlyDictionary<string, string> GetAllLabels()
        {
            return _labelsMap;
        }
        
        #endregion
        
        private DateTime Now()
        {
            return DateTime.Now;
        }
    }
}