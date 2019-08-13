using System.Collections.Generic;

namespace FrameworkSDK.Constructing
{
    public static class DefaultConfigurationSteps
    {
        public const string Initialization = "initialization";
        public static class InitializationActions
        {
            public const string Localization = "localization";
            public const string Logging = "logging";
            public const string Ioc = "ioc";
        }

        public const string Register = "register";
        public static class RegisterActions
        {
            public const string Core = "core";
        }

        public const string Constructing = "constructing";
        public static class ConstructingActions
        {
            public const string Container = "container";
            public const string Game = "game";
        }

        public static Dictionary<string, IReadOnlyList<string>> GetSortedList()
        {
            return new Dictionary<string, IReadOnlyList<string>>()
            {
                {
                    Initialization,
                    new []
                    {
                        InitializationActions.Localization,
                        InitializationActions.Logging,
                        InitializationActions.Ioc
                    }
                }, 
            };
        }
    }
}
