using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Console.Core.Implementations.ExternalProcess.ProcessMessages
{
    public class BinaryConsoleProcessMessageFormatter : IConsoleProcessMessageFormatter
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();
        
        public byte[] Serialize(IConsoleProcessMessage message)
        {
            using (var steam = new MemoryStream())
            {
                _binaryFormatter.Serialize(steam, message);
                return steam.ToArray();
            }
        }

        public IConsoleProcessMessage Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return (IConsoleProcessMessage)_binaryFormatter.Deserialize(stream);
            }
        }
    }
}