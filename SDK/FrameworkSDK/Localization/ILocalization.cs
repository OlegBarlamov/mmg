using System.Globalization;

namespace FrameworkSDK.Localization
{
    public interface ILocalization
    {
        CultureInfo Culture { get; }

        string GetString(string key);
    }
}