using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Mapping;
using FrameworkSDK.Game.Views;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using NetExtensions;

namespace FrameworkSDK.Game.Scenes
{
	public abstract class Scene : IScene, IDisposable
	{
		public string Name { get; }
		
		[NotNull] private ModuleLogger Logger { get; }
		[NotNull] private IMvcStrategyService MvcStrategy { get; }
		[NotNull, ItemNotNull] private UpdatableCollection<IController> Controllers { get; }
		[NotNull, ItemNotNull] private UpdatableCollection<ViewMapping> Views { get; }

		protected Scene([NotNull] string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));

			MvcStrategy = AppContext.ServiceLocator.Resolve<IMvcStrategyService>();

			Logger = new ModuleLogger(FrameworkLogModule.Scenes);
			Controllers = new UpdatableCollection<IController>();
			Views = new UpdatableCollection<ViewMapping>();
		}

		protected Scene()
			:this(NamesGenerator.Hash(HashType.SmallGuid, nameof(Scene).ToLowerInvariant()))
		{
		}

	    public virtual void Dispose()
	    {

	    }

		public override string ToString()
		{
			return Name;
		}

		public void AddController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			Logger.Info(Strings.Info.AddControllerToScene, controller.Name, Name);
			CheckOwner(controller);

			var scheme = MvcStrategy.ResolveByController(controller);
			if (scheme.Controller != null)
				AddControllerInternal(scheme.Controller);
		}

		public void RemoveController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			Logger.Info(Strings.Info.RemovedControllerFromScene, controller.Name, Name);
			CheckOwner(controller);
			if (!Controllers.Contains(controller))
			{
				throw new NotImplementedException();
			}

			Controllers.Remove(controller);
			OnControllerDetachedInternal(controller);
			OnControllerDetached(controller);
		}

		public void AddView([NotNull] IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));

			var scheme = MvcStrategy.ResolveByView(view);
			if (scheme.Controller != null)
				AddControllerInternal(scheme.Controller);
			else
			{
				//separate view
				AddView(view, null);
			}
		}

		public void RemoveSingleView([NotNull] IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));

			var targetView = Views.FirstOrDefault(mapping => mapping.View == view);
			if (targetView == null)
			{
				throw new NotImplementedException();
			}

			//TODO check for correct!
			Views.Remove(targetView);
		}

		public void AddController([NotNull] object model)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));

			var scheme = MvcStrategy.ResolveByModel(model);
			if (scheme.Controller != null)
				AddControllerInternal(scheme.Controller);
		}

		public void RemoveController([NotNull] object model)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));

			var targetController = Controllers.FirstOrDefault(controller => controller.IsOwnedModel(model));
			if (targetController != null)
				RemoveController(targetController);
		}

		public void ClearControllers()
		{
			Logger.Info(Strings.Info.RemovingAllControllersFromScene, Name);

			var count = 0;
			var names = new List<string>();
			foreach (var controller in Controllers)
			{
				OnControllerDetachedInternal(controller);
				OnControllerDetached(controller);
				count++;
				names.Add(controller.Name);
			}
			Controllers.Clear();

			Logger.Info(Strings.Info.RemovedMultipleControllersFromScene, names.ToArray().ArrayToString(), count, Name);
		}

		void IUpdatable.Update(GameTime gameTime)
		{
			Controllers.Update();
			Views.Update();

			foreach (var controller in Controllers)
			{
				controller.Update(gameTime);
			}

			Update(gameTime);
		}

		void IDrawable.Draw(GameTime gameTime)
		{
			//TODO чистка фоном и т.п

			var views = Views.Select(mapping => mapping.View);
			foreach (var view in views)
			{
				view.Draw(gameTime);
			}
		}

		void IClosable.OnClosed()
		{


			OnClosed();
		}

		void IScene.OnOpened()
		{
			OnOpened();
		}

		ClosingState IUpdatable<ClosingState>.UpdateState(GameTime gameTime)
		{
			return ClosingUpdate(gameTime);
		}

		protected virtual ClosingState ClosingUpdate([NotNull] GameTime gameTime)
		{
			return new ClosingState { CanClose = true };
		}

		protected virtual void OnClosed()
		{
			
		}

		protected virtual void OnOpened()
		{

		}

		protected virtual void Update([NotNull] GameTime gameTime)
		{
			
		}

		protected virtual void OnControllerAttached([NotNull] IController controller)
		{
			
		}

		protected virtual void OnControllerDetached([NotNull] IController controller)
		{
			
		}

		private void AddControllerInternal([NotNull] IController controller)
		{
			Controllers.Add(controller);

			OnControllerAttachedInternal(controller);
			OnControllerAttached(controller);
		}

		private void CheckOwner([NotNull] ISceneComponent sceneComponent)
		{
			if (sceneComponent == null) throw new ArgumentNullException(nameof(sceneComponent));

			if (sceneComponent.OwnedScene != null && sceneComponent.OwnedScene != this)
				throw new ScenesException(string.Format(Strings.Exceptions.Scenes.SceneComponentWrongOwner, sceneComponent, this));
		}

		private void OnControllerAttachedInternal(IController controller)
		{
			if (controller.View != null)
				AddView(controller.View, controller);
		}

		private void OnControllerDetachedInternal(IController controller)
		{
			var targetMapping = Views.FirstOrDefault(mapping => mapping.IsMappedController(controller));
			if (targetMapping != null)
				RemoveView(targetMapping);
		}

		private void RemoveView(ViewMapping viewMapping)
		{
			var controller = viewMapping.Controller;
			var view = viewMapping.View;

			CheckOwner(view);
			Views.Remove(viewMapping);

			view.Destroy();

			Logger.Info(Strings.Info.DestroyViewFromScene, view.Name, controller?.Name, Name);
		}

		private void AddView([NotNull] IView view, [CanBeNull] IController controller)
		{
			CheckOwner(view);
			var mapping = new ViewMapping(view, controller);
			Views.Add(mapping);

			Logger.Info(Strings.Info.AddViewToScene, view.Name, controller?.Name, Name);
		}
	}
}
