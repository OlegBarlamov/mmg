namespace FrameworkSDK.Localization
{
    public static class Strings
    {
        public static ILocalization Localization { get; set; }

        public static class Info
        {
            public static string LogRegistered => Localization.GetString();
            public static string LocalizationRegistered => Localization.GetString();
            public static string IoCRegistered => Localization.GetString();
        }

        public static class Exceptions
        {
            public static string AppInitialization => Localization.GetString();
            public static string ConstructionStateFinished => Localization.GetString();
        }
    }
}
