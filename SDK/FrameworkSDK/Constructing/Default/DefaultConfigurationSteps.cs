// ReSharper disable once CheckNamespace
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

        public const string ExternalRegistration = "registration_external";
        public static class ExternalRegistrationActions
        {
            public const string Registration = "registration";
        }

        public const string Constructing = "constructing";
        public static class ConstructingActions
        {
            public const string Core = "core";
        }

        public static class ContextKeys
	    {
		    public static readonly string Localization = "localization";
		    public static readonly string Logger = "logger";
		    public static readonly string Ioc = "ioc";
	        public static readonly string Container = "container";
            public static readonly string BaseLogger = "base_logger";
		    public static readonly string Locator = "locator";
        }
    }
}
