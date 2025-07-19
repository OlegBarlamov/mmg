using System;
using System.Collections.Generic;
using System.Security.Claims;
using Epic.Server.Objects;

namespace Epic.Server.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static void FillWithSessionData(this ClaimsIdentity identity, ISessionObject session)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                new Claim(ClaimTypes.Name, session.UserId.ToString()),
                
                new Claim("SessionId", session.Id.ToString()),
                new Claim("Token", session.Token),
                new Claim("Created", session.Created.ToString("O")),
                new Claim("LastAccessed", session.LastAccessed.ToString("O")),
                new Claim("IsRevoked", session.IsRevoked.ToString()),
                new Claim("DeviceInfo", session.DeviceInfo ?? "Unknown"),
                new Claim("IpAddress", session.IpAddress ?? "Unknown"),
                new Claim("UserAgent", session.UserAgent ?? "Unknown")
            };
            if (session.PlayerId.HasValue)
                claims.Add(new Claim("PlayerId", session.PlayerId.Value.ToString()));
            
            identity.AddClaims(claims);
        }
        
        public static string GetSessionToken(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Token")?.Value;
        }
        
        public static Guid GetId(this ClaimsPrincipal principal)
        {
            return Guid.Parse((ReadOnlySpan<char>)principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public static bool TryGetPlayerId(this ClaimsPrincipal principal, out Guid playerId)
        {
            var id = GetPlayerId(principal);
            playerId = id ?? Guid.Empty;
            return id.HasValue;
        }

        public static Guid? GetPlayerId(this ClaimsPrincipal principal)
        {
            var playerId = principal.FindFirst("PlayerId")?.Value;
            if (playerId != null)
                return Guid.Parse(playerId);
            
            return null;
        }
    }
}