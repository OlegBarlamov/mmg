using System;
using Atom.Client.AppComponents;
using Atom.Client.Resources;
using Console.FrameworkAdapter;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.Resources.Generation;
using Logging.FrameworkAdapter;
using NetExtensions.Geometry;

namespace Atom.Client.Windows
{
    internal class Program
    {
#if DEBUG
        private const bool IsDebug = true;
#else
        private const bool IsDebug = false;
#endif
        
        [STAThread]
        public static void Main()
        {
            var gameParameters = new DefaultGameParameters
            {
                BackBufferSize = new SizeInt(1280, 768),
                IsMouseVisible = false,
            };
            using (var gameFactory = X4GameFactory.GetFactory(IsDebug, gameParameters).Construct())
            {
                gameFactory.Run();
            }
        }
    }
}