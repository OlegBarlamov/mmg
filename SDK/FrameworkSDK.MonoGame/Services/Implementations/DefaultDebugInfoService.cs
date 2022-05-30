using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    [UsedImplicitly]
    internal class DefaultDebugInfoService : IDebugInfoService
    {
        private readonly ConcurrentDictionary<string, DateTime> _startMeasuringTimes = new ConcurrentDictionary<string, DateTime>();
        private readonly ConcurrentDictionary<string, TimeSpan> _resultMeasuringTimes = new ConcurrentDictionary<string, TimeSpan>();

        private readonly ConcurrentDictionary<string, DateTime> _startTimers = new ConcurrentDictionary<string, DateTime>();
        
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

        private DateTime Now()
        {
            return DateTime.Now;
        }
    }
}