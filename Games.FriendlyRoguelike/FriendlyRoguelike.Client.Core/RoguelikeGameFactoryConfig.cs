namespace FriendlyRoguelike.Core
{
    public class RoguelikeGameFactoryConfig
    {
        public LogginCoreConfig LogginCoreConfig { get; }

        public RoguelikeGameFactoryConfig()
        {
            LogginCoreConfig = new LogginCoreConfig();
        }
    }

    public class LogginCoreConfig
    {
        public string LogDirectoryFullPath { get; set; } = "Logs";
        public bool IsDebug { get; set; } = false;
        public bool FakeLog { get; set; } = false;
    }
}