using System;
using FrameworkSDK.IoC;

namespace TablePlatform.Client.MacOS
{
    internal class Program : IAppRunProgram
    {
        [STAThread]
        public static void Main()
        {
            using (var game = TablePlatformFactory.Create(new Program()))
            {
                game.Run();
            }
        }

        public void RegisterCustomServices(IServiceRegistrator serviceRegistrator)
        {
        }
    }
}