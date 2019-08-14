using System;
using FrameworkSDK.Common;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.Game
{
    public abstract class Controller : IController
    {
        public string Name { get; protected set; }

        IView IController.View => _view;

        object IController.Model => _model;
        
        private object _model;
        private IView _view;

        Scene ISceneComponent.OwnedScene => _ownedScene;

        private Scene _ownedScene;

        protected Controller() : this(NamesGenerator.Hash(HashType.SmallGuid, nameof(Controller)))
        {
        }

        protected Controller([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public virtual void Update(GameTime gameTime)
        {
        }

	    public override string ToString()
	    {
		    return Name;
	    }

	    bool IController.IsOwnedModel(object model)
        {
            return ReferenceEquals(model, _model);
        }

        protected virtual void SetModel([NotNull] object model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        protected virtual void SetView([NotNull] IView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        void IController.SetOwner([NotNull] Scene ownedScene)
        {
            _ownedScene = ownedScene ?? throw new ArgumentNullException(nameof(ownedScene));
        }

        void IController.SetModel(object model)
        {
            if (_model == null)
                SetModel(model);
        }

        void IController.SetView(IView view)
        {
            if (_view == null)
                SetView(view);
        }
    }
}