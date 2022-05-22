using FrameworkSDK.Localization;

namespace FrameworkSDK.MonoGame.Localization
{
    internal static class Strings
    {
        public static ILocalization Localization { get; set; }

        public static class Info
        {
            public static class Mapping
            {
                public static string ResolvingMvcByModel => Localization.GetString() ?? "Resolving mvc by model '{0}'.";
                public static string ResolvingMvcByController => Localization.GetString() ?? "Resolving mvc by controller '{0}'.";
                public static string ResolvingMvcByView => Localization.GetString() ?? "Resolving mvc by view '{0}'.";
                public static string ResolvingMvcByModelFinished => Localization.GetString() ?? "Resolving by model {0} finished with result: {1}.";
                public static string ResolvingMvcByControllerFinished => Localization.GetString() ?? "Resolving by controller {0} finished with result: {1}.";
                public static string ResolvingMvcByViewFinished => Localization.GetString() ?? "Resolving by view {0} finished with result: {1}.";
                public static string ControllerExists => Localization.GetString() ?? "Controller already exists in {0}.";
                public static string ModelExists => Localization.GetString() ?? "Model already exists in {0}.";
                public static string ViewExists => Localization.GetString() ?? "View already exists in {0}.";
                public static string SceneRegisteredForModel => Localization.GetString() ?? "Scene {0} registered for model {1}.";
                public static string SceneResolvedByModel => Localization.GetString() ?? "Scene {0} resolved by model {1}.";
            }

            public static class Resources
            {
	            public static string StartLoadingResourcePackage => Localization.GetString() ?? "Loading resource package {0} started.";
	            public static string FinishLoadingResourcePackage => Localization.GetString() ?? "Resource package {0} loaded.";
	            public static string StartUnloadingResourcePackage => Localization.GetString() ?? "Unloading resource package {0} started.";
	            public static string FinishedUnloadingResourcePackage => Localization.GetString() ?? "Resource package {0} unloaded.";
            }

            public static class Cameras
            {
	            public static string CameraSwitched => Localization.GetString() ?? "Active camera switched from {0} to {1}.";
            }
            
            public static string SceneSwitchingState => Localization.GetString() ?? "Scenes switching from {0} to {1}.";
	        public static string SceneSwitched => Localization.GetString() ?? "Scenes switched from {0} to {1}.";

	        public static string AddControllerToScene => Localization.GetString() ?? "Adding controller {0} to scene {1}.";
	        public static string RemovedControllerFromScene => Localization.GetString() ?? "Removing controller {0} from scene {1}.";
	        public static string RemovingAllControllersFromScene => Localization.GetString() ?? "Removing all controllers from scene {0}.";
	        public static string RemovedMultipleControllersFromScene => Localization.GetString() ?? "Controllers:\"{0}\"(total:{1}) removed from scene {2}.";

			public static string AddViewToScene => Localization.GetString() ?? "View {0} for controller {1} added to scene {2}.";
	        public static string DestroyViewFromScene => Localization.GetString() ?? "View {0} for controller {1} removed from scene {3}.";
        }
        
        public static class Warnings
        {
	        public static class Resources
	        {
		        public static string ResourcePackageNotUnloaded =>
			        Localization.GetString() ?? "Resource package {0} not unloaded.";
	        }
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

	        public static class Resources
	        {
		        public static string ResourceAssetsReferencedByAnotherPackage =>
			        Localization.GetString() ?? "Package {0}. {1} resources assets referenced by multiple package. Package will not be unloaded.";
	        }
        }

        public static class Exceptions
        {
	        public static string FatalException => Localization.GetString() ?? "Fatal error! Game will be closed.";
	        
	        public static string AppContextNotInitialized => Localization.GetString() ?? "Application context not initialized.";
	        
	        public static string GameHeartServicesNotInitialized => Localization.GetString() ?? "Game heart services not initialized.";

	        public static class Constructing
	        {
		        public static string ConstructingFailed => Localization.GetString() ?? "Constructing game {0} failed. See inner exception for more info.";
		        public static string RunGameFailed => Localization.GetString() ?? "Running game {0} failed. See inner exception for more info.";
		        public static string MultipleGameInstances => Localization.GetString() ?? "Creating the second instance of {0}. Multiple instance creation of game app not allowed.";
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
                public static string ErrorWhileResolvingScene => Localization.GetString() ?? "Error while resolving scene by model {0}.";
            }

            public static class Scenes
	        {
				public static string SceneComponentWrongOwner => Localization.GetString() ?? "Сompoentnt {0} has already added to another scene and can not be added to {1}.";
	            public static string ControllerAlreadyExists => Localization.GetString() ?? "Controller {0} has already exists in scene {1}.";
	            public static string ViewAlreadyExists => Localization.GetString() ?? "View {0} has already exists in scene {1}.";
                public static string ControllerNotExists => Localization.GetString() ?? "Controller {0} not exists in scene {1}.";
	            public static string ControllerForModelNotExists => Localization.GetString() ?? "Controller for model {0} not exists in scene {1}.";
	            public static string ControllerForModelNotExistsValidateFalse => Localization.GetString() ?? "Controller for model {0} not exists in scene {1}. Validate result was incorrect!";
	            public static string ViewForModelNotExists => Localization.GetString() ?? "View for model {0} not exists in scene {1}.";
	            public static string ViewForModelNotExistsValidateFalse => Localization.GetString() ?? "View for model {0} not exists in scene {1}. Validate result was incorrect!";
                public static string ViewNotExists => Localization.GetString() ?? "View {0} not exists in scene {1}.";
	            public static string SceneComponentNotAttached => Localization.GetString() ?? "Scene component {0} not attached to any scene.";
	            public static string ChildComponentNotExists => Localization.GetString() ?? "Scene component {0} not contains child component {1}.";
            }

            public static class Resources
            {
	            public static string LoadResourceFailed => Localization.GetString() ?? "Load resource {0} failed.";
	            public static string UnloadResourceFailed => Localization.GetString() ?? "Unload resource {0} failed.";
            }
            
            public static class Graphics
            {
	            public static string PipelineAlreadyBuilt => Localization.GetString() ?? "Pipeline already built.";
            }
        }
    }
}