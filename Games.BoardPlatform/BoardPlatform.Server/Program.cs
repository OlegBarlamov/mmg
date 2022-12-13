using AspNetCore.FrameworkAdapter;
using Console.FrameworkAdapter.Constructing;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BoardPlatform.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var appBuilder = WebApplication.CreateBuilder(args);
            appBuilder.Services.AddControllers();
            
            var app = appBuilder.UseFramework()
                .AddServices<ConsoleCommandsExecutingServicesModule>()
                .AddServices<ServerServicesModule>()
                .AddComponent<TerminalConsoleSubsystem>()
                .Construct()
                .Asp();
            
            app.MapControllers();
            app.UseWebSockets()
                .UseHttpsRedirection()
                .UseAuthorization();

            app.Run();
        }
    }
}