using System;
using Console.FrameworkAdapter;
using Console.FrameworkAdapter.Constructing;
using Console.GameFrameworkAdapter.Constructing;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.Resources.Generation;
using Logging.FrameworkAdapter;
using NetExtensions.Geometry;

namespace Omegas.Client.MacOs
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
                BackBufferSize = new SizeInt(1920, 1080),
                IsMouseVisible = false,
            };
            using (var gameFactory = OmegasGameFactory.GetFactory(IsDebug, gameParameters).Construct())
            {
                gameFactory.Run();
            }
        }
    }
}