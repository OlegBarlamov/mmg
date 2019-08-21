using System;
using Epic.Core.Logging;
using FrameworkSDK;
using FrameworkSDK.Constructing;

namespace Epic.Game
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var logFactory = new LogFactory("Logs", true);
            var module = new CommonModule(logFactory);

            using (var app = App.Construct()
                .UseGameFramework<TestApplication>()
                .SetupCustomLogger(logFactory.CreateAdapter)
                .RegisterServices(module.Register))
            {
                app.Run();
            }
		}
    }
#endif
}
