namespace FrameworkSDK.Localization
{
    internal static class Strings
    {
        public static ILocalization Localization { get; set; }

        public static class Info
        {
            public static class AppConstructing
            {
                public static string DefaultServices => Localization.GetString() ?? "Default services registered.";
                public static string AppRunning => Localization.GetString() ?? "App launched.";

                public static string ConstructingStart => Localization.GetString() ?? "App constructing started.";
                public static string ConstructingStep => Localization.GetString() ?? "Step: {0}.";
                public static string ConstructingAction => Localization.GetString() ?? "Action: {0}.";
                public static string ConstructingEnd => Localization.GetString() ?? "App constructing finished.";

                public static string ServiceLocatorCreated => Localization.GetString() ?? "Service locator '{0}' has been created. Registrations: {1}";
                public static string ServiceLocatorInstanceCreated => Localization.GetString() ?? "Service locator '{0}' created the instance by {1}.";
            }
        }

	    public static class Errors
	    {
	    }

		public static class Exceptions
        {
            public static string FatalException => Localization.GetString() ?? "Fatal error! Application will be closed.";
            public static string AppContextNotInitialized => Localization.GetString() ?? "Application context not initialized.";
            
            public static string ObjectAlreadyInitialized => Localization.GetString() ?? "Object '{0}' has initialized already.";
            public static string ObjectNotInitialized => Localization.GetString() ?? "Object '{0}' has not initialized yet.";

            public static class Ioc
            {
	            public static string NoPublicConstructorsException => Localization.GetString() ?? "Public constructors not founded for type {0}.";
	            public static string DependencyNotResolvedException => Localization.GetString() ?? "Dependency {0} can not be resolved.";
	            public static string TypeNotRegisteredException => Localization.GetString() ?? "Type {0} not registered in {1}.";
	            public static string ResolvingTypeException => Localization.GetString() ?? "Can not resolve type {0}.";
	            public static string DisposeServicesException => Localization.GetString() ?? "Error while dispose services in locator.";
	            public static string NoSuitablecConstructorsException => Localization.GetString() ?? "Suitable constructors for type {0} not founded. Unresolved type: {1}.";
	            public static string NoSuitablecConstructorsExceptionWithParameters => Localization.GetString() ?? "Suitable constructors for type {0} not founded with additional parameters {1}. Unresolved type: {2}.";
	            public static string BadResolveStrategy => Localization.GetString() ?? "Resolving with additional parameters can't have Singleton registering type";
            }
            
            public static class Constructing
            {
                public static string ObjectInContextNotFound => Localization.GetString() ?? "Object {0} not found in pipeline context.";
				public static string FactoryObjectNull => Localization.GetString() ?? "Factory received in configuration step, processed null object.";
				public static string StepNotFound => Localization.GetString() ?? "Pipeline step {0} not founded.";
                public static string ActionFailed => Localization.GetString() ?? "Action {0} failed.";
                public static string StepFailed => Localization.GetString() ?? "Step {0} failed.";
	            public static string ConstructingFailed => Localization.GetString() ?? "Constructing app {0} failed. See inner exception for more info.";
	            public static string RunAppFailed => Localization.GetString() ?? "Running app {0} failed. See inner exception for more info.";
			}

            public static class Pipeline
            {
                public static string ActionNotFound => Localization.GetString() ?? "Pipeline action {0} not founded.";
				public static string ActionAlreadyExists => Localization.GetString() ?? "Pipeline action {0} already exist.";
				public static string StepNotFound => Localization.GetString() ?? "Pipeline step {0} not founded.";
			}
        }
    }
}
