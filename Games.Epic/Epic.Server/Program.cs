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
             // appBuilder.Services.AddCors(options =>
             // {
             //     options.AddPolicy("AllowLocalhost3000",
             //         policy =>
             //         {
             //             policy.WithOrigins("http://localhost:3000")
             //                 .AllowAnyHeader()
             //                 .AllowAnyMethod();
             //         });
             // });
 
             var app = appBuilder.UseFramework()
                 .AddServices<ConsoleCommandsExecutingServicesModule>()
                 .AddServices<ServerServicesModule>()
                 .AddComponent<TerminalConsoleSubsystem>()
                 .AddComponent<DebugStartupScript>()
                 .Construct()
                 .Asp();

             
             //app.UseCors("AllowLocalhost3000");
             app.UseMiddleware<RequestResponseLoggingMiddleware>();
             app.UseRouting();
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