using System.Globalization;
using FrameworkSDK.Properties;

namespace FrameworkSDK.Localization
{
    public class DefaultLocalization : ILocalization
    {
        public CultureInfo Culture { get; } = CultureInfo.CurrentCulture;

        public DefaultLocalization()
        {
            Resources.Culture = Culture;
        }

        public string GetString(string key)
        {
            return Resources.ResourceManager.GetString(key);
        }
    }
}
