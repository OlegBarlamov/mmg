using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using NetExtensions.Collections;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
	public abstract class SceneBase : IScene, IDisposable
	{
		public string Name { get; }
		
		public virtual bool ReadyToBeClosed { get; protected set; }
		
        protected object Model { get; private set; }
        
        [NotNull, ItemNotNull] protected ObservableList<IGraphicComponent> GraphicComponents { get; } = new ObservableList<IGraphicComponent>();

        [NotNull] private ModuleLogger Logger { get; }
		[NotNull] private IMvcStrategyService MvcStrategy { get; }
		[NotNull, ItemNotNull] private UpdatableCollection<IController> Controllers { get; }
		[NotNull, ItemNotNull] private UpdatableCollection<ViewMapping> Views { get; }

		IGraphicsPipeline IScene.GraphicsPipeline => GetGraphicsPipeline();

			object IScene.DataModel
	    {
	        get => Model;
	        set => Model = value;
	    }

	    protected SceneBase([NotNull] string name, object model = null)
	    {
	        Name = name ?? throw new ArgumentNullException(nameof(name));

	        Model = model;
            MvcStrategy = AppContext.ServiceLocator.Resolve<IMvcStrategyService>();

            Logger = new ModuleLogger(LogCategories.Mvc);
	        Controllers = new UpdatableCollection<IController>();
	        Views = new UpdatableCollection<ViewMapping>();
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

            if (Controllers.Contains(controller))
                throw new ScenesException(Strings.Exceptions.Scenes.ControllerAlreadyExists, controller, this);

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
                throw new ScenesException(Strings.Exceptions.Scenes.ControllerNotExists, controller, this);

			Controllers.Remove(controller);
			OnControllerDetachedInternal(controller);
			OnControllerDetached(controller);
		}

	    [NotNull] public IController AddController([NotNull] object model)
	    {
	        if (model == null) throw new ArgumentNullException(nameof(model));

	        var validate = MvcStrategy.ValidateByModel(model);
            if (!validate.IsControllerExist)
                throw new ScenesException(Strings.Exceptions.Scenes.ControllerForModelNotExists, model, this);

            var scheme = MvcStrategy.ResolveByModel(model);
	        if (scheme.Controller == null)
	            throw new ScenesException(Strings.Exceptions.Scenes.ControllerForModelNotExistsValidateFalse, model, this);

	        AddControllerInternal(scheme.Controller);
	        return scheme.Controller;
        }

	    [NotNull] public IController RemoveController([NotNull] object model)
	    {
	        if (model == null) throw new ArgumentNullException(nameof(model));

	        var targetController = Controllers.Find(controller => controller.IsOwnedDataModel(model));
	        if (targetController != null)
	        {
	            RemoveController(targetController);
	            return targetController;
	        }

            throw new ScenesException(Strings.Exceptions.Scenes.ControllerForModelNotExists, model, this);
	    }

	    [CanBeNull]
	    public IController FindControllerByActiveModel(object model)
	    {
	        return Controllers.Find(controller => controller.IsOwnedDataModel(model));
	    }

        public void AddView([NotNull] IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));

            if (Views.ContainsView(view))
                throw new ScenesException(Strings.Exceptions.Scenes.ViewAlreadyExists, view, this);

            var scheme = MvcStrategy.ResolveByView(view);
			if (scheme.Controller != null)
				AddControllerInternal(scheme.Controller);
			else
			{
				//separate view
				AddView(view, null, scheme.Model);
			}
		}

	    public IView AddView([NotNull] object model)
	    {
	        if (model == null) throw new ArgumentNullException(nameof(model));

	        var validate = MvcStrategy.ValidateByModel(model);
	        if (!validate.IsViewExist)
	            throw new ScenesException(Strings.Exceptions.Scenes.ViewForModelNotExists, model, this);

	        var scheme = MvcStrategy.ResolveByModel(model);
	        if (scheme.View == null)
	            throw new ScenesException(Strings.Exceptions.Scenes.ViewForModelNotExistsValidateFalse, model, this);

	        if (scheme.Controller != null)
	            AddControllerInternal(scheme.Controller);
	        else
	            AddView(scheme.View, null, scheme.Model);

	        return scheme.View;
        }

	    public void RemoveView([NotNull] IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));

            if (!Views.ContainsView(view))
                throw new ScenesException(Strings.Exceptions.Scenes.ViewNotExists, view, this);

            //TODO can be optimized by using hashtable
			var targetView = Views.First(mapping => mapping.View == view);
            if (targetView.Controller != null)
                RemoveController(targetView.Controller);
            else
			    RemoveView(targetView);
		}

	    public IView RemoveView([NotNull] object model)
	    {
		    if (model == null) throw new ArgumentNullException(nameof(model));
		    
		    var validate = MvcStrategy.ValidateByModel(model);
		    if (!validate.IsViewExist)
			    throw new ScenesException(Strings.Exceptions.Scenes.ViewForModelNotExists, model, this);
		    
		    //TODO can be optimized by using hashtable
		    var targetView = Views.Find(mapping => mapping.Model == model);
		    if (targetView == null)
			    throw new ScenesException(Strings.Exceptions.Scenes.ViewForModelNotExists, model, this);
		    
		    if (targetView.Controller != null)
			    RemoveController(targetView.Controller);
		    else 
				RemoveView(targetView);

		    return targetView.View;
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

		    Controllers.Update(gameTime);

            Update(gameTime);
		}

		void IClosable.OnClosed()
		{
			OnClosed();
		}

	    void IScene.OnOpened()
		{
			OnOpened();
		}

	    void IScene.OnOpening()
	    {
		    OnOpening();
	    }

	    void IClosable.CloseRequest()
		{
			CloseRequest();
		}

	    protected virtual IGraphicsPipeline GetGraphicsPipeline()
	    {
		    return GraphicsPipeline.Empty;
	    }

	    protected virtual void OnFirstOpening()
	    {
		    
	    }

	    protected virtual void CloseRequest()
		{
			ReadyToBeClosed = true;
		}

		protected virtual void OnClosed()
		{
			ReadyToBeClosed = false;
		}

		protected virtual void OnOpened()
		{

		}
		
		protected virtual void OnOpening()
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
		    if (Controllers.Contains(controller))
		        throw new ScenesException(Strings.Exceptions.Scenes.ControllerAlreadyExists, controller, this);

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
				AddView(controller.View, controller, controller.DataModel);

            controller.OnAddedToScene(this);
		}

		private void OnControllerDetachedInternal(IController controller)
		{
			var targetMapping = Views.Find(mapping => mapping.IsMappedController(controller));
			if (targetMapping != null)
				RemoveView(targetMapping);

            controller.OnRemovedFromScene(this);
		}

		private void RemoveView(ViewMapping viewMapping)
		{
			var controller = viewMapping.Controller;
			var view = viewMapping.View;

			CheckOwner(view);
			Views.Remove(viewMapping);

		    Logger.Info(Strings.Info.DestroyViewFromScene, view.Name, controller?.Name, Name);

		    GraphicComponents.Remove(view);
		    view.OnRemovedFromScene(this);

            view.Destroy();
		}

		private void AddView([NotNull] IView view, [CanBeNull] IController controller, [CanBeNull] object model)
		{
			CheckOwner(view);
			var mapping = new ViewMapping(view, controller, model);
            if (Views.ContainsView(view))
                throw new ScenesException(Strings.Exceptions.Scenes.ViewAlreadyExists, view, this);

			Views.Add(mapping);

			Logger.Info(Strings.Info.AddViewToScene, view.Name, controller?.Name, Name);

			GraphicComponents.Add(view);
		    mapping.View.OnAddedToScene(this);
        }

	    protected static string GenerateSceneName()
	    {
	        return NamesGenerator.Hash(HashType.SmallGuid, nameof(SceneBase).ToLowerInvariant());
	    }
	}
}
