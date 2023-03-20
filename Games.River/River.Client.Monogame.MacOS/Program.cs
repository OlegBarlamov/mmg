using System;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using NetExtensions.Geometry;

namespace River.Client.MacOS
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using (var game = new DefaultAppFactory(args)
                .AddServices(new MainModule())
                .UseGame<RiverGameApp>()
                .UseGameParameters(new DefaultGameParameters
                {
                    BackBufferSize = new SizeInt(1024, 768)
                })
                .PreloadResourcePackage(new TilesResourcePackage())
                .UseMvc()
                .Construct())
            {
                game.Run();
            }
        }
    }
}