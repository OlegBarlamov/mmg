using System;
using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Scenes
{
	internal class ViewMapping
	{
	    [NotNull] public IView View { get; }
		[CanBeNull] public IController Controller { get; }

		public ViewMapping([NotNull] IView view, [CanBeNull] IController controller)
		{
			View = view ?? throw new ArgumentNullException(nameof(view));
		    Controller = controller;
		}

		public bool IsMappedController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			return ReferenceEquals(Controller, controller);
		}
	}
}
