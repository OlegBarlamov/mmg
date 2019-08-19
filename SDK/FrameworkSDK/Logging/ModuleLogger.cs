using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    internal class ModuleLogger : IFrameworkLogger, IDisposable
    {
        [NotNull] private IFrameworkLogger FrameworkLogger { get; }

        private FrameworkLogModule LogModule { get; }

	    [CanBeNull] private MessageInfo _lastMessageInfo;

        private static readonly IFormatProvider DefaultFormatProvider = new NullFormatProvider();

		public ModuleLogger([NotNull] IFrameworkLogger frameworkLogger, FrameworkLogModule logModule)
        {
            FrameworkLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));

            LogModule = logModule;
        }

	    public ModuleLogger(FrameworkLogModule logModule) 
			: this(AppContext.Logger, logModule)
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
	        LogInternal(message, LogModule, level);
        }

		void IFrameworkLogger.Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
			LogInternal(message, LogModule, level);
		}

	    private void LogInternal(string message, FrameworkLogModule module, FrameworkLogLevel level)
		{
			var info = new MessageInfo(message, module, level);
			if (info.Equals(_lastMessageInfo))
			{
				_lastMessageInfo.Increment();
				return;
			}

			if (_lastMessageInfo?.Count > 1)
				_lastMessageInfo.Output(FrameworkLogger);

			_lastMessageInfo = info;
			FrameworkLogger.Log(message, module, level);
		}

	    private class MessageInfo : IEquatable<MessageInfo>
	    {
			public int Count { get; private set; }

		    private string Message { get; }
		    private FrameworkLogModule Module { get; }
		    private FrameworkLogLevel LogLevel { get; }

		    public MessageInfo([NotNull] string message, FrameworkLogModule module, FrameworkLogLevel logLevel)
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
				    hashCode = (hashCode * 397) ^ (int) Module;
				    hashCode = (hashCode * 397) ^ (int) LogLevel;
				    return hashCode;
			    }
		    }
	    }

	    public void Dispose()
	    {
	        if (_lastMessageInfo?.Count > 1)
	            _lastMessageInfo.Output(FrameworkLogger);
        }
    }
}
