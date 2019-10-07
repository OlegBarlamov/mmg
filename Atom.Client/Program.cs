using System;
using Atom.Client.Logging;
using Atom.Client.Services;
using Atom.Client.Services.Implementations;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Constructing;
using Microsoft.Extensions.Logging;

namespace Atom.Client
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
			using (var app = ConsoleApp.Create<TestConsoleApp>())
	        {
		        app.Run();
	        }
		}
    }
#endif
}
