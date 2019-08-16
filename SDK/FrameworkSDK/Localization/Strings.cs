namespace FrameworkSDK.Localization
{
    public static class Strings
    {
        public static ILocalization Localization { get; set; }

        public static class Info
        {
            public static class AppConstructing
            {
                public static string DefaultServices => Localization.GetString() ?? "default services registered.";
                public static string AppRunning => Localization.GetString() ?? "app running...";

                public static string ConstructingStart => Localization.GetString() ?? "app constructing start.";
                public static string ConstructingStep => Localization.GetString() ?? "constructing step {0}";
                public static string ConstructingAction => Localization.GetString() ?? "constructing action {0}";
                public static string ConstructingEnd => Localization.GetString() ?? "app constructing end.";
            }

	        public static string SceneSwitchingState => Localization.GetString() ?? "scenes switching from {0} to {1}";
	        public static string SceneSwitched => Localization.GetString() ?? "scenes switched from {0} to {1}";

	        public static string AddControllerToScene => Localization.GetString() ?? "controller {0} has added to scene {1}";
	        public static string RemovedControllerFromScene => Localization.GetString() ?? "controller {0} has removed from scene {1}";
	        public static string RemovingAllControllersFromScene => Localization.GetString() ?? "removing all controllers from scene {0}";
	        public static string RemovedMultipleControllersFromScene => Localization.GetString() ?? "controllers:\"{0}\"(total:{1}) have removed from scene {2}";

			public static string AddViewToScene => Localization.GetString() ?? "view {0} for controller {1} has added to scene {2}";
	        public static string DestroyViewFromScene => Localization.GetString() ?? "view {0} for controller {1} has destroyed from scene {3}";
		}

	    public static class Errors
	    {
			public static string SceneChangingWhileNotAllowed => Localization.GetString() ?? "attempt to change scene to {0} while that not available";
		}


		public static class Exceptions
        {
            public static string FatalException => Localization.GetString() ?? "Fatal error! Application will be closed.";
            public static string AppContextNotInitialized => Localization.GetString() ?? "Application context not initialized.";

            public static class Ioc
	        {
		        public static string NoPublicConstructorsException => Localization.GetString() ?? "public constructors not founded for type {0}";
		        public static string DependencyNotResolvedException => Localization.GetString() ?? "Dependency {0} can not be resolved";
				public static string TypeNotRegisteredException => Localization.GetString() ?? "Type {0} not registered";
				public static string ResolvingTypeException => Localization.GetString() ?? "Can not resolve type {0}";
				public static string DisposeServicesException => Localization.GetString() ?? "Error while dispose services in locator";
				public static string NoSuitablecConstructorsException => Localization.GetString() ?? "Have not founded suitable constructor for type {0}";
		        public static string NoSuitablecConstructorsExceptionWithParameters => Localization.GetString() ?? "Have not founded suitable constructor for type {0}";
				public static string BadResolveStrategy => Localization.GetString() ?? "Resolving with parameter can not have Singleton registering type";
			}

            public static class Constructing
            {
                public static string ObjectInContextNotFound => Localization.GetString() ?? $"object {0} not found in pipeline context";
				public static string FactoryObjectNull => Localization.GetString() ?? "factory gotted in configuration step = null";
				public static string StepNotFound => Localization.GetString() ?? "Pipeline step {0} not founded";
                public static string ActionFailed => Localization.GetString() ?? "action {0} failed";
                public static string StepFailed => Localization.GetString() ?? "step {0} failed";
            }

            public static class Pipeline
            {
                public static string ActionNotFound => Localization.GetString() ?? "Pipeline action {0} not founded";
				public static string ActionAlreadyExists => Localization.GetString() ?? "Pipeline action {0} already exist";
				public static string StepNotFound => Localization.GetString() ?? "Pipeline step {0} not founded";
			}

            public static class Mapping
            {
                public static string ControllerForModelNotFound => Localization.GetString() ?? "Controller for model {0} not founded";
				public static string ControllerCreationError => Localization.GetString() ?? "Error while creating controller";
				public static string ViewForModelNotFound => Localization.GetString() ?? "View for model {0} not founded";
				public static string ViewCreationError => Localization.GetString() ?? "Error while creating view";
				public static string IncompatibleModelType => Localization.GetString() ?? "Incompatible model type. Expected {0} but was {1}";
				public static string IncompatibleControllerType => Localization.GetString() ?? "Incompatible controller type. Expected {0} but was {1}";
				public static string IncompatibleViewType => Localization.GetString() ?? "Incompatible view type. Expected {0} but was {1}";
			}

            public static class Scenes
	        {
				public static string SceneComponentWrongOwner => Localization.GetString() ?? "compoentnt {0} has already added to another scene and can not be added to {1}";
			}
		}
    }
}
