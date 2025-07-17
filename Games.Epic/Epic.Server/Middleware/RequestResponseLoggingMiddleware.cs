using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Epic.Server.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Skip WebSocket upgrade requests
            if (context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }
            
            // Skip static file requests based on path or extension
            if (IsStaticAssetRequest(context.Request.Path))
            {
                await _next(context); // No logging
                return;
            }
            
            // Log Request
            context.Request.EnableBuffering(); // Important!
            var requestBody = await ReadStreamAsync(context.Request.Body);
            _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path} - Body: {requestBody}");
            context.Request.Body.Position = 0;

            // Swap response stream
            var originalBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Continue through the pipeline
            await _next(context);

            // Skip logging if switching protocols (like WebSocket)
            if (context.Response.StatusCode == StatusCodes.Status101SwitchingProtocols)
                return;
            
            // Read and log response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            if (IsTextOrJson(context.Response.ContentType))
            {
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                _logger.LogInformation($"Response: {context.Response.StatusCode} - Body: {responseText}");
            }
            else
            {
                _logger.LogInformation($"Response: {context.Response.StatusCode} - [Skipped non-text content]");
            }
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static async Task<string> ReadStreamAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            stream.Position = 0;
            return body;
        }
        
        private static bool IsTextOrJson(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            contentType = contentType.ToLowerInvariant();

            return contentType.StartsWith("application/json") ||
                   contentType.StartsWith("text/");
        }
        
        private static bool IsStaticAssetRequest(PathString path)
        {
            string[] extensionsToSkip = new[]
            {
                ".js", ".css", ".ico", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".woff", ".woff2", ".ttf", ".eot", ".map"
            };

            var value = path.Value?.ToLowerInvariant();
            if (string.IsNullOrEmpty(value))
                return false;

            foreach (var ext in extensionsToSkip)
            {
                if (value.EndsWith(ext))
                    return true;
            }

            return false;
        }
    }

}