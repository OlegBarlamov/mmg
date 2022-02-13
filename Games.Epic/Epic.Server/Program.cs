using AspNetCore.FrameworkAdapter;
using FrameworkSDK;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Epic.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new DefaultAppFactory(args)
                .UseAspNetCore(CreateHostBuilder(args))
                .Construct()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}