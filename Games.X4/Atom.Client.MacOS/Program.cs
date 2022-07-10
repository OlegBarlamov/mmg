using System;
using FrameworkSDK.MonoGame.Config;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS
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