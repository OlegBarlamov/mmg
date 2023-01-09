using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BoardPlatform.Data
{
    public interface IRawWidgetData
    {
        object GetValue(string key);
    }
    
    public static class RawWidgetDataExtension
    {
        public static T GetValue<T>(this IRawWidgetData widgetData, string key)
        {
            return (T) widgetData.GetValue(key);
        }
    }

    public class DictionaryBasedRawWidgetData : IRawWidgetData
    {
        private readonly ConcurrentDictionary<string, object> _data;
        
        public DictionaryBasedRawWidgetData() : this(new Dictionary<string, object>())
        {
        }

        private DictionaryBasedRawWidgetData(IDictionary<string, object> data)
        {
            _data = new ConcurrentDictionary<string, object>(data);
        }
        
        public void AddValue(string key, object value)
        {
            _data.AddOrUpdate(key, value, (s, o) => o);
        }
        
        public object GetValue(string key)
        {
            return _data[key];
        }

        public static DictionaryBasedRawWidgetData FromDictionary(IDictionary<string, object> data)
        {
            return new DictionaryBasedRawWidgetData(data);
        }
    } 
}