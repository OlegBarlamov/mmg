using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Services
{
    public interface IDebugInfoService
    {
        void StartMeasure(string key);
        void StopMeasure(string key);
        TimeSpan GetMeasure(string key);
        IReadOnlyDictionary<string, TimeSpan> GetAllMeasures();
        
        
        void StartTimer(string key);
        TimeSpan GetTimer(string key);
        IReadOnlyDictionary<string, DateTime> GetAllTimers();


        void IncrementCounter(string key);
        void DecrementCounter(string key);
        void SetCounter(string key, int value);
        int GetCounter(string key);
        IReadOnlyDictionary<string, int> GetAllCounters();

        void SetLabel(string key, string label);
        IReadOnlyDictionary<string, string> GetAllLabels();
    }
}