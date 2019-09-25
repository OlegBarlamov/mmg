namespace FrameworkSDK.MonoGame.Constructing
{
    internal static class GameDefaultConfigurationSteps
    {
        public const string Initialization = "initialization";
        public static class InitializationActions
        {
            
        }

	    public const string BaseSetup = "base_setup";
	    public static class BaseSetupActions
		{
			
		}

		public const string Registration = "registration";
        public static class RegistrationActions
		{
            public const string Game = "gameHeart";
		    public const string GameParameters = "game_parameters";
        }

        public const string ExternalRegistration = "registration_external";
        public static class ExternalRegistrationActions
        {
            }

        public const string Constructing = "constructing";
        public static class ConstructingActions
        {
            public const string Game = "gameHeart";
        }

        public static class ContextKeys
	    {
            //default
	        public static readonly string Localization = "localization";
	        public static readonly string Logger = "logger";
	        public static readonly string Ioc = "ioc";
	        public static readonly string Container = "container";
	        public static readonly string BaseLogger = "base_logger";
	        public static readonly string Locator = "locator";

            public static readonly string Game = "gameHeart";
	        public static readonly string Host = "host";
        }
    }
}
