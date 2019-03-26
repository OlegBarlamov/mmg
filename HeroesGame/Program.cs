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
			var appSettings = new ApplicationSettings
			{
				GameName = "Heroes",
				LogDirectoryPath = "Logs"
			};

			using (var app = new Application<GameRoot>(appSettings))
			{
				app.Start<RootModule>();
			}
		}
    }
#endif
}
