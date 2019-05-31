using System.Globalization;
using JetBrains.Annotations;

namespace FrameworkSDK.Localization
{
    internal class LocalizationShell : ILocalization
    {
        public CultureInfo Culture => CustomLocalization?.Culture ?? _defaultLocalization.Culture;

        public bool IsUsedCustomLocalization => CustomLocalization != null;

        [CanBeNull] private ILocalization CustomLocalization { get; set; }

        [NotNull] private readonly ILocalization _defaultLocalization = new DefaultLocalization();

        public string GetString(string key)
        {
            return CustomLocalization?.GetString(key) ?? _defaultLocalization.GetString(key);
        }

        public void SetupLocalization([CanBeNull] ILocalization localization)
        {
            CustomLocalization = localization;
        }
    }
}
