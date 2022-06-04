using System;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	internal class ViewMapping
	{
	    [NotNull] public IView View { get; }
		[CanBeNull] public IController Controller { get; }
		[CanBeNull] public object Model { get; }

		public ViewMapping([NotNull] IView view, [CanBeNull] IController controller, [CanBeNull] object model)
		{
			View = view ?? throw new ArgumentNullException(nameof(view));
		    Controller = controller;
		    Model = model;
		}

		public bool IsMappedController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			return ReferenceEquals(Controller, controller);
		}
	}
}
