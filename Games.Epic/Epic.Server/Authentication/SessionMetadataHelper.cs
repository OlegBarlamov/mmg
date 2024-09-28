using System;
using Epic.Server.Objects;
using Microsoft.AspNetCore.Http;

namespace Epic.Server.Authentication
{
    public static class SessionMetadataHelper
    {
        public static SessionMetadata ExtractFromContext(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var ipAddress = context.Connection.RemoteIpAddress;
            var ipAddressString = ipAddress != null ? ipAddress.ToString() : "Unknown IP";
            var deviceInfo = ParseUserAgent(userAgent);

            return new SessionMetadata
            {
                DeviceInfo = deviceInfo,
                IpAddress = ipAddressString,
                UserAgent = userAgent,
            };
        }

        private static IDeviceInfo ParseUserAgent(string userAgent)
        {
            var deviceInfo = new MutableDeviceInfo
            {
                UserAgent = userAgent,
                IsMobile = false,
                IsTablet = false,
                IsDesktop = true // Default to desktop
            };

            // Check for mobile devices
            if (userAgent.Contains("Mobi", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.IsMobile = true;
                deviceInfo.IsDesktop = false; // If it's mobile, it's not desktop
            }

            // Check for tablet devices
            if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.IsTablet = true;
                deviceInfo.IsMobile = false; // If it's a tablet, it's not mobile
            }

            // Parse browser information
            if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.Browser = "Chrome";
                deviceInfo.BrowserVersion = ExtractVersion(userAgent, "Chrome");
            }
            else if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.Browser = "Firefox";
                deviceInfo.BrowserVersion = ExtractVersion(userAgent, "Firefox");
            }
            else if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.Browser = "Safari";
                deviceInfo.BrowserVersion = ExtractVersion(userAgent, "Safari");
            }
            else if (userAgent.Contains("Edge", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.Browser = "Edge";
                deviceInfo.BrowserVersion = ExtractVersion(userAgent, "Edge");
            }

            // Parse operating system information
            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.OperatingSystem = "Windows";
                deviceInfo.OperatingSystemVersion = ExtractVersion(userAgent, "Windows");
            }
            else if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.OperatingSystem = "MacOS";
                deviceInfo.OperatingSystemVersion = ExtractVersion(userAgent, "Macintosh");
            }
            else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.OperatingSystem = "Linux";
                deviceInfo.OperatingSystemVersion = ExtractVersion(userAgent, "Linux");
            }
            else if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.OperatingSystem = "Android";
                deviceInfo.OperatingSystemVersion = ExtractVersion(userAgent, "Android");
            }
            else if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
                     userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                deviceInfo.OperatingSystem = "iOS";
                deviceInfo.OperatingSystemVersion = ExtractVersion(userAgent, "iOS");
            }

            return deviceInfo;
        }
        
        private static string ExtractVersion(string userAgent, string platform)
        {
            var versionIndex = userAgent.IndexOf(platform, StringComparison.OrdinalIgnoreCase);
            if (versionIndex != -1)
            {
                var versionStart = versionIndex + platform.Length + 1; // Skip the platform name and a space
                var versionEnd = userAgent.IndexOfAny(new[] { ' ', ';', ')' }, versionStart); // Find the next space or semicolon
                if (versionEnd == -1)
                    versionEnd = userAgent.Length; // Go to end if not found

                return userAgent.Substring(versionStart, versionEnd - versionStart).Trim();
            }
            return string.Empty;
        }
    }
}