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

        IView IController.View { get; set; }

        object IController.Model
        {
            get => _model;
            set => _model = value;
        }

        private object _model;

        Scene ISceneComponent.OwnedScene => _ownedScene;

        private Scene _ownedScene;

        protected Controller() : this(NamesGenerator.Hash(HashType.SmallGuid, nameof(Controller)))
        {
        }

        protected Controller([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(GameTime gameTime)
        {
        }

        bool IController.IsOwnedModel(object model)
        {
            return ReferenceEquals(model, _model);
        }

        void IController.SetOwner([NotNull] Scene ownedScene)
        {
            _ownedScene = ownedScene ?? throw new ArgumentNullException(nameof(ownedScene));
        }
    }
}