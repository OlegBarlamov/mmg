using System;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;

namespace HeroesGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var app = App.Construct()
                .UseGameFramework<TestApplication>()
                .SetupCustomLogger(() => new ConsoleLogger()))
            {
                app.Run();
            }
		}
    }

    internal class ConsoleLogger : IFrameworkLogger
    {
        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            Console.WriteLine($@"{level}:{module}:{message}");
        }
    }
#endif
}
