namespace FrameworkSDK.Localization
{
    public static class Strings
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
            }

	        public static string SceneSwitchingState => Localization.GetString() ?? "Scenes switching from {0} to {1}.";
	        public static string SceneSwitched => Localization.GetString() ?? "Scenes switched from {0} to {1}.";

	        public static string AddControllerToScene => Localization.GetString() ?? "Adding controller {0} to scene {1}.";
	        public static string RemovedControllerFromScene => Localization.GetString() ?? "Сontroller {0} removed from scene {1}.";
	        public static string RemovingAllControllersFromScene => Localization.GetString() ?? "Removing all controllers from scene {0}.";
	        public static string RemovedMultipleControllersFromScene => Localization.GetString() ?? "Controllers:\"{0}\"(total:{1}) removed from scene {2}.";

			public static string AddViewToScene => Localization.GetString() ?? "View {0} for controller {1} added to scene {2}.";
	        public static string DestroyViewFromScene => Localization.GetString() ?? "View {0} for controller {1} removed from scene {3}.";
		}

	    public static class Errors
	    {
			public static string SceneChangingWhileNotAllowed => Localization.GetString() ?? "Attempt to change scene to {0} while it was forbidden.";

		    public static class Mapping
		    {
			    public static string ResolvingControllerFailed =>
				    Localization.GetString() ?? "Resolving controllers by {0} failed.";
			    public static string ResolvingViewFailed =>
				    Localization.GetString() ?? "Resolving view by {0} failed.";
			    public static string ResolvingModelFailed =>
				    Localization.GetString() ?? "Resolving model by {0} failed.";
			}
	    }

		public static class Exceptions
        {
            public static string FatalException => Localization.GetString() ?? "Fatal error! Application will be closed.";
            public static string AppContextNotInitialized => Localization.GetString() ?? "Application context not initialized.";

            public static class Ioc
	        {
		        public static string NoPublicConstructorsException => Localization.GetString() ?? "Public constructors not founded for type {0}.";
		        public static string DependencyNotResolvedException => Localization.GetString() ?? "Dependency {0} can not be resolved.";
				public static string TypeNotRegisteredException => Localization.GetString() ?? "Type {0} not registered.";
				public static string ResolvingTypeException => Localization.GetString() ?? "Can not resolve type {0}.";
				public static string DisposeServicesException => Localization.GetString() ?? "Error while dispose services in locator.";
				public static string NoSuitablecConstructorsException => Localization.GetString() ?? "Suitable constructors for type {0} not founded.";
		        public static string NoSuitablecConstructorsExceptionWithParameters => Localization.GetString() ?? "Suitable constructors for type {0} not founded with additional parameters {1}.";
				public static string BadResolveStrategy => Localization.GetString() ?? "Resolving with additional parameters can't have Singleton registering type";
			}

            public static class Constructing
            {
                public static string ObjectInContextNotFound => Localization.GetString() ?? "Object {0} not found in pipeline context.";
				public static string FactoryObjectNull => Localization.GetString() ?? "Factory received in configuration step, processed null object.";
				public static string StepNotFound => Localization.GetString() ?? "Pipeline step {0} not founded.";
                public static string ActionFailed => Localization.GetString() ?? "Action {0} failed.";
                public static string StepFailed => Localization.GetString() ?? "Step {0} failed.";
            }

            public static class Pipeline
            {
                public static string ActionNotFound => Localization.GetString() ?? "Pipeline action {0} not founded.";
				public static string ActionAlreadyExists => Localization.GetString() ?? "Pipeline action {0} already exist.";
				public static string StepNotFound => Localization.GetString() ?? "Pipeline step {0} not founded.";
			}

            public static class Mapping
            {
                public static string MappingForInstanceNotFound => Localization.GetString() ?? "Mapping for instance {0} not founded.";
				public static string ControllerCreationError => Localization.GetString() ?? "Error while resolving controller.";
				public static string ViewCreationError => Localization.GetString() ?? "Error while resolving view.";
	            public static string ModelCreationError => Localization.GetString() ?? "Error while resolving model.";
				public static string IncompatibleModelType => Localization.GetString() ?? "Incompatible model type. Expected {0} but was {1}.";
				public static string IncompatibleControllerType => Localization.GetString() ?? "Incompatible controller type. Expected {0} but was {1}.";
				public static string IncompatibleViewType => Localization.GetString() ?? "Incompatible view type. Expected {0} but was {1}.";
			}

            public static class Scenes
	        {
				public static string SceneComponentWrongOwner => Localization.GetString() ?? "Сompoentnt {0} has already added to another scene and can not be added to {1}.";
			}
		}
    }
}
