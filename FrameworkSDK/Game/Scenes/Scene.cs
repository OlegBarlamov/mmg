using System;
using FrameworkSDK.Common;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace FrameworkSDK.Game.Scenes
{
	public abstract class Scene : IScene, IDisposable
	{
		public string Name { get; }
		
		[NotNull] private ModuleLogger Logger { get; }

		private UpdatableCollection<IController> Controllers { get; }

		protected Scene([NotNull] string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));

			Logger = new ModuleLogger(FrameworkLogModule.Scenes);
			Controllers = new UpdatableCollection<IController>();
		}

		protected Scene()
			:this(NamesGenerator.Hash(HashType.SmallGuid, nameof(Scene).ToLowerInvariant()))
		{
		}

		void IControllersManager.AddController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			Logger.Info(Strings.Info.AddControllerToScene, controller.Name, Name);
			Controllers.Add(controller);

			OnControllerAttachedInternal(controller);
			OnControllerAttached(controller);
		}

		void IControllersManager.RemoveController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			Logger.Info(Strings.Info.RemovedControllerFromScene, controller.Name, Name);
			Controllers.Remove(controller);

			OnControllerDetachedInternal(controller);
			OnControllerDetached(controller);
		}

		void IUpdatable.Update(GameTime gameTime)
		{
			Controllers.Update();

			foreach (var controller in Controllers)
			{
				controller.Update(gameTime);
			}

			Update(gameTime);
		}

		void IDrawable.Draw(GameTime gameTime)
		{

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

		public virtual void Dispose()
		{
			
		}

		private void OnControllerAttachedInternal(IController controller)
		{
			
		}

		private void OnControllerDetachedInternal(IController controller)
		{

		}
	}
}
