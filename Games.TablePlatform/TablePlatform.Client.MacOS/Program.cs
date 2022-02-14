using System;
using FrameworkSDK.DependencyInjection;
using TablePlatform.DesktopClient;

namespace TablePlatform.Client.MacOS
{
    internal class Program : IAppRunProgram
    {
        [STAThread]
        public static void Main()
        {
            using (var game = TablePlatformFactory.Create(new Program()).Construct())
            {
                game.Run();
            }
        }

        public void RegisterCustomServices(IServiceRegistrator serviceRegistrator)
        {
        }
    }
}