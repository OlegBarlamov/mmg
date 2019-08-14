using System.Runtime.CompilerServices;

namespace FrameworkSDK.Localization
{
    public static class LocalizationExtension
    {
        public static string GetString(this ILocalization localization, [CallerMemberName] string key = "")
        {
            return localization.GetString(key) ?? string.Empty;
        }
    }
}
