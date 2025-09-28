using AspNetCore.FrameworkAdapter;
using Console.FrameworkAdapter.Constructing;
using Epic.Server.Middleware;
using FrameworkSDK.Constructing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Epic.Server
 {
     public class Program
     {
         public static void Main(string[] args)
         {
             var appBuilder = WebApplication.CreateBuilder(args);
             appBuilder.Services.AddControllers();
 
             var app = appBuilder.UseFramework()
                 .AddServices<ConsoleCommandsExecutingServicesModule>()
                 .AddServices<ServerServicesModule>()
                 .AddComponent<TerminalConsoleSubsystem>()
                 .AddComponent<InitializeResourcesScript>()
                 .AddComponent<InitializeUnitsScript>()
                 .AddComponent<InitializeRewardDefinitionsScript>()
                 .AddComponent<DebugStartupScript>()
                 .Construct()
                 .Asp();

             
             app.UseMiddleware<RequestResponseLoggingMiddleware>();
             app.UseRouting();
             app.UseStaticFiles();
             // app.UseHttpsRedirection();
             app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api"), a =>
             {
                 a.UseMiddleware<AuthMiddleware>();
             });
             app.UseWebSockets();
             app.UseEndpoints(endpoints =>
             {
                 endpoints.MapControllers();
             });
             app.UseSpa(spa =>
             {
                 spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
             });
 
             app.Run();
         }
     }
 }