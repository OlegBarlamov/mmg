using System;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Scenes
{
	internal class ViewMapping
	{
		public IView View { get; }
		public IController Controller { get; }

		public ViewMapping([NotNull] IView view, [NotNull] IController controller)
		{
			View = view ?? throw new ArgumentNullException(nameof(view));
			Controller = controller ?? throw new ArgumentNullException(nameof(controller));
		}

		public bool IsMappedController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			return ReferenceEquals(Controller, controller);
		}
	}
}
