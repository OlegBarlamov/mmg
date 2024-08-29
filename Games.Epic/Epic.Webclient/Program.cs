using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Epic.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            app.MapGet("/", context => Task.FromResult("Hello World!"));
            app.Run();
        }
    }
}


