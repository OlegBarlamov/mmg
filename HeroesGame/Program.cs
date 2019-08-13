using System;
using FrameworkSDK;

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
            using (var app = App.Construct<TestApplication>())
            {
                app.Run();
            }
		}
    }
#endif
}
