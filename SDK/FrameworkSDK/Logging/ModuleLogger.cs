using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    public class ModuleLogger : IFrameworkLogger, IDisposable
    {
        [NotNull] private IFrameworkLogger CoreLogger { get; }

        private string Category { get; }

	    [CanBeNull] private MessageInfo _lastMessageInfo;

        private static readonly IFormatProvider DefaultFormatProvider = new NullFormatProvider();

		public ModuleLogger([NotNull] IFrameworkLogger frameworkLogger, string category)
        {
            CoreLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));

            Category = category;
        }

	    public ModuleLogger(string category) 
			: this(AppContext.Logger, category)
	    {
		    
	    }


		public void Info(string message, params object[] args)
        {
            Log(string.Format(DefaultFormatProvider, message, args), FrameworkLogLevel.Info);
        }

        public void Trace(string message, params object[] args)
        {
            Log(string.Format(DefaultFormatProvider, message, args), FrameworkLogLevel.Trace);
        }

        public void Debug(string message, params object[] args)
        {
            Log(string.Format(DefaultFormatProvider, message, args), FrameworkLogLevel.Debug);
        }

        public void Warn(string message, params object[] args)
        {
            Log(string.Format(DefaultFormatProvider, message, args), FrameworkLogLevel.Warn);
        }

        public void Error(string message, params object[] args)
        {
            Log(string.Format(DefaultFormatProvider, message, args), FrameworkLogLevel.Error);
        }

	    public void Error(string message, Exception exception, params object[] args)
	    {
		    message = $"{message}: {exception}";
		    Log(string.Format(DefaultFormatProvider, message, args), FrameworkLogLevel.Error);
	    }

		public void Fatal(string message, params object[] args)
        {
            Log(string.Format(DefaultFormatProvider, message, args), FrameworkLogLevel.Fatal);
        }

        public void Log(string message, FrameworkLogLevel level = FrameworkLogLevel.Info, params object[] args)
        {
	        LogInternal(message, Category, level);
        }

        public void LogCollection<T>([NotNull, ItemNotNull] IEnumerable<T> collection, FrameworkLogLevel level = FrameworkLogLevel.Info)
        {
	        if (collection == null) throw new ArgumentNullException(nameof(collection));
	        LogCollection(collection, x => x.ToString(), level);
        }
        
        public void LogCollection<T>([NotNull, ItemNotNull] IEnumerable<T> collection, [NotNull] Func<T, string> formatter,
	        FrameworkLogLevel level = FrameworkLogLevel.Info)
        {
	        if (collection == null) throw new ArgumentNullException(nameof(collection));
	        if (formatter == null) throw new ArgumentNullException(nameof(formatter));
	        foreach (var item in collection)
	        {
		        var msg = formatter.Invoke(item);
		        Log(msg, level);
	        }
        }

		void IFrameworkLogger.Log(string message, string category, FrameworkLogLevel level)
        {
			LogInternal(message, category, level);
		}

	    private void LogInternal(string message, string module, FrameworkLogLevel level)
		{
			var info = new MessageInfo(message, module, level);
			if (info.Equals(_lastMessageInfo))
			{
				_lastMessageInfo.Increment();
				return;
			}

			if (_lastMessageInfo?.Count > 1)
				_lastMessageInfo.Output(CoreLogger);

			_lastMessageInfo = info;
			CoreLogger.Log(message, module, level);
		}

	    private class MessageInfo : IEquatable<MessageInfo>
	    {
			public int Count { get; private set; }

		    private string Message { get; }
		    private string Module { get; }
		    private FrameworkLogLevel LogLevel { get; }

		    public MessageInfo([NotNull] string message, string module, FrameworkLogLevel logLevel)
		    {
			    Message = message ?? throw new ArgumentNullException(nameof(message));
			    Module = module;
			    LogLevel = logLevel;
			    Count = 1;
		    }

		    public void Increment()
		    {
			    Count++;
		    }

		    public void Output(IFrameworkLogger logger)
		    {
			    logger.Log(Count > 1 ? $"{Message} (repeted {Count} times)" : Message, Module, LogLevel);
		    }

		    public bool Equals(MessageInfo other)
		    {
			    if (ReferenceEquals(null, other)) return false;
			    if (ReferenceEquals(this, other)) return true;
			    return string.Equals(Message, other.Message) && Module == other.Module && LogLevel == other.LogLevel;
		    }

		    public override bool Equals(object obj)
		    {
			    if (ReferenceEquals(null, obj)) return false;
			    if (ReferenceEquals(this, obj)) return true;
			    if (obj.GetType() != this.GetType()) return false;
			    return Equals((MessageInfo) obj);
		    }

		    public override int GetHashCode()
		    {
			    unchecked
			    {
				    var hashCode = (Message != null ? Message.GetHashCode() : 0);
				    hashCode = (hashCode * 397) ^ Module.GetHashCode();
				    hashCode = (hashCode * 397) ^ (int) LogLevel;
				    return hashCode;
			    }
		    }
	    }

	    public void Dispose()
	    {
	        if (_lastMessageInfo?.Count > 1)
	            _lastMessageInfo.Output(CoreLogger);
        }
    }
}
