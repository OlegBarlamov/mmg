namespace FrameworkSDK.Localization
{
    public static class Strings
    {
        public static ILocalization Localization { get; set; }

        public static class Info
        {
	        public static string DefaultServices => Localization.GetString();
			public static string ConstructingStart => Localization.GetString();
	        public static string ConstructingEnd => Localization.GetString();
	        public static string AppRunning => Localization.GetString();

	        public static string SceneSwitchingState => Localization.GetString();
	        public static string SceneSwitched => Localization.GetString();

	        public static string AddControllerToScene => Localization.GetString();
	        public static string RemovedControllerFromScene => Localization.GetString();
	        public static string RemovingAllControllersFromScene => Localization.GetString();
	        public static string RemovedMultipleControllersFromScene => Localization.GetString();

			public static string AddViewToScene => Localization.GetString();
	        public static string DestroyViewFromScene => Localization.GetString();
		}

	    public static class Errors
	    {
			public static string SceneChangingWhileNotAllowed => Localization.GetString();
		}


		public static class Exceptions
        {
            public static string FatalException => Localization.GetString();

            public static class Ioc
	        {
		        public static string NoPublicConstructorsException => Localization.GetString();
		        public static string DependencyNotResolvedException => Localization.GetString();
		        public static string TypeNotRegisteredException => Localization.GetString();
		        public static string ResolvingTypeException => Localization.GetString();
		        public static string DisposeServicesException => Localization.GetString();
		        public static string NoSuitablecConstructorsException => Localization.GetString();
	            public static string BadResolveStrategy => Localization.GetString();
            }

            public static class Constructing
            {
                public static string ObjectInContextNotFound => Localization.GetString();
                public static string FactoryObjectNull => Localization.GetString();
                public static string StepNotFound => Localization.GetString();
            }

            public static class Pipeline
            {
                public static string ActionNotFound => Localization.GetString();
                public static string ActionAlreadyExists => Localization.GetString();
                public static string StepNotFound => Localization.GetString();
            }

            public static class Mapping
            {
                public static string ControllerForModelNotFound => Localization.GetString();
                public static string ControllerCreationError => Localization.GetString();
                public static string ViewForModelNotFound => Localization.GetString();
                public static string ViewCreationError => Localization.GetString();
                public static string IncompatibleModelType => Localization.GetString();
                public static string IncompatibleControllerType => Localization.GetString();
                public static string IncompatibleViewType => Localization.GetString();
            }

            public static class Scenes
	        {
				public static string SceneComponentWrongOwner => Localization.GetString();
			}

		}
    }
}
