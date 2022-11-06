using System;
using AspNetCore.FrameworkAdapter;
using FrameworkSDK.Constructing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BoardPlatform.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var factory = new AspNetCoreAppFactory(args);
            factory.AddServices<ServerServicesModule>();
            factory.Services.AddControllers();

            var app = factory.Construct().Asp();
            app.UseWebSockets();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            
            app.Run();
        }

        private static void Func()
        {
            while (true)
            {
                var line = Console.ReadLine();
                Console.WriteLine("Hi: " + line);
            }
        }
    }
}