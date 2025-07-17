using System;
using System.Threading.Tasks;
using Epic.Server.Authentication;
using Epic.Server.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using IAuthorizationService = Epic.Server.Services.IAuthorizationService;

namespace Epic.Server.Middleware
{
    public class AuthMiddleware
    {
        private ISessionsService SessionsService { get; }
        private IAuthorizationService AuthorizationService { get; }
        public ILogger<AuthMiddleware> Logger { get; }
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next, [NotNull] ISessionsService sessionsService,
            [NotNull] IAuthorizationService authorizationService, [NotNull] ILogger<AuthMiddleware> logger)
        {
            SessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
            AuthorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Cookies.TryGetValue(AuthenticationConstants.AuthenticationTokenCookieName,
                    out var token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Login required.\"}");
                return;
            }

            var validationResult = await SessionsService.ValidateToken(token);
            if (!validationResult.IsValid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("{\"error\": \" Token is " + validationResult.Reason + ".\"}");
                return;
            }

            var session = validationResult.Session;
            context.User = await AuthorizationService.AuthorizeAsync(session);

            await _next(context);
        }
    }
}