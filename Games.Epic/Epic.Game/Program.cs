using System;
using Epic.Core.Logging;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Constructing;

namespace Epic.Game
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var logFactory = new LogFactory("Logs", true);
            var module = new CommonModule(logFactory, logFactory);

            var appFactory = new AppFactory();
            using (var app = appFactory.CreateGame<TestApplication>()
                .SetupCustomLogger(logFactory.CreateAdapter)
                .RegisterServices(module.Register)
                .Configure())
            {
                app.Run();
            }
		}
    }
#endif
}
