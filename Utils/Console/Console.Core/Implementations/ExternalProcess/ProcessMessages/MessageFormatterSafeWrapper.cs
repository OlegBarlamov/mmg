using System;
using JetBrains.Annotations;

namespace Console.Core.Implementations.ExternalProcess.ProcessMessages
{
    public class MessageFormatterSafeWrapper : IConsoleProcessMessageFormatter
    {
        private IConsoleProcessMessageFormatter Formatter { get; }
        private Action<Exception, IConsoleProcessMessage> OnSerializeError { get; }
        private Action<Exception> OnDeserializeError { get; }

        public MessageFormatterSafeWrapper([NotNull] IConsoleProcessMessageFormatter formatter, [NotNull] Action<Exception, IConsoleProcessMessage> onSerializeError,
            [NotNull] Action<Exception> onDeserializeError)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            OnSerializeError = onSerializeError ?? throw new ArgumentNullException(nameof(onSerializeError));
            OnDeserializeError = onDeserializeError ?? throw new ArgumentNullException(nameof(onDeserializeError));
        }
        
        public byte[] Serialize(IConsoleProcessMessage message)
        {
            try
            {
                return Formatter.Serialize(message);
            }
            catch (Exception e)
            {
                OnSerializeError(e, message);
                return null;
            }
        }

        public IConsoleProcessMessage Deserialize(byte[] data)
        {
            try
            {
                return Formatter.Deserialize(data);
            }
            catch (Exception e)
            {
                OnDeserializeError(e);
                return null;
            }
        }
    }
}