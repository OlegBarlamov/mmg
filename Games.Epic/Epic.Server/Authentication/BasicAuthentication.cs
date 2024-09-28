using System;
using Microsoft.AspNetCore.Http;

namespace Epic.Server.Authentication
{
    public static class BasicAuthentication
    {
        public static string[] ExtractCredentials(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
                return Array.Empty<string>();

            var authHeader = request.Headers["Authorization"].ToString();
            if (!authHeader.StartsWith("Basic "))
                return Array.Empty<string>();

            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials))
                .Split(':');
        }

        public static string GetHashFromCredentials(string userName, string password)
        {
            return "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userName}:{password}"));
        }
    }
}