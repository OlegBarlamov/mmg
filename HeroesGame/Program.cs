using System;

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
			using (var app = new TestApplication())
			{
				app.Run();
			}
		}
    }
#endif
}
