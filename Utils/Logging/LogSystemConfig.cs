using System.IO;

namespace Logging
{
    public class LogSystemConfig
    {
        public DirectoryInfo LogDirectory { get; set; } 
        
        public bool IsDebug { get; set; }
    }
}