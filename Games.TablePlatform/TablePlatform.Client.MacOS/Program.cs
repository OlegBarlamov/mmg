using System;
using FrameworkSDK.IoC;

namespace TablePlatform.Client.MacOS
{
    internal class Program : IAppRunProgram
    {
        [STAThread]
        public static void Main()
        {
            var program = new Program();
            using (var game = TablePlatformFactory.Create(program))
            {
                game.Run();
            }
        }

        public void RegisterCustomServices(IServiceRegistrator serviceRegistrator)
        {
            
        }
    }
}