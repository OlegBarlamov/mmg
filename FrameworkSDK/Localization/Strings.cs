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
            public static string SubsystemsFound => Localization.GetString();
            public static string SubsystemInitialize => Localization.GetString();

	        public static string DefaultServices => Localization.GetString();
			public static string ConstructingStart => Localization.GetString();
	        public static string ConstructingEnd => Localization.GetString();
	        public static string BuildContainer => Localization.GetString();
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
            public static string AppInitialization => Localization.GetString();
            public static string ConstructionStateFinished => Localization.GetString();
            public static string SubsystemInitializeException => Localization.GetString();
            public static string FatalException => Localization.GetString();

            public static class Ioc
	        {
		        public static string NoPublicConstructorsException => Localization.GetString();
		        public static string DependencyNotResolvedException => Localization.GetString();
		        public static string TypeNotRegisteredException => Localization.GetString();
		        public static string ResolvingTypeException => Localization.GetString();
		        public static string DisposeServicesException => Localization.GetString();
		        public static string NoSuitablecConstructorsException => Localization.GetString();
			}

            public static class Mapping
            {
                public static string ControllerForModelNotFound => Localization.GetString();
                public static string ControllerCreationError => Localization.GetString();
                public static string ViewForModelNotFound => Localization.GetString();
                public static string ViewCreationError => Localization.GetString();
            }

            public static class Scenes
	        {
				public static string SceneComponentWrongOwner => Localization.GetString();
			}

		}
    }
}
