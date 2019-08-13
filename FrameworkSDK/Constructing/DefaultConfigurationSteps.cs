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

	    public const string BaseSetup = "base_setup";
	    public static class BaseSetupActions
		{
			public const string Setup = "setup";
		}

		public const string Registration = "registration";
        public static class RegistrationActions
		{
            public const string Core = "core";
		}

        public const string Constructing = "constructing";
        public static class ConstructingActions
        {
            public const string Core = "core";
        }

	    public const string Run = "run";

	    public static class ContextKeys
	    {
		    public static readonly string Localization = "localization";
		    public static readonly string Logger = "logger";
		    public static readonly string ServiceContainer = "ioc";
		    public static readonly string BaseLogger = "base_logger";
		    public static readonly string Locator = "locator";
	    }

		public static Dictionary<string, IReadOnlyList<string>> GetSortedList()
        {
            return new Dictionary<string, IReadOnlyList<string>>
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
	            {
		            BaseSetup,
					new []
					{
						BaseSetupActions.Setup
					}
	            },
	            {
		            Registration,
		            new []
		            {
			            RegistrationActions.Core
					}
	            },
	            {
		            Constructing,
		            new []
		            {
			            ConstructingActions.Core
		            }
	            }
			};
        }
    }
}
