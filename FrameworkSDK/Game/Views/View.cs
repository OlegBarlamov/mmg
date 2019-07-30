using System;
using FrameworkSDK.Common;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Scenes;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game.Views
{
    public abstract class View : IView
    {
        public string Name { get; protected set; }

        Scene ISceneComponent.OwnedScene => _ownedScene;

        object IView.DataModel
        {
            get => _dataModel;
            set => _dataModel = value;
        }

        IController IView.Controller
        {
            get => _controller;
            set => _controller = value;
        }

        private object _dataModel;
        private IController _controller;
        private Scene _ownedScene;

        protected View() : this(NamesGenerator.Hash(HashType.SmallGuid, nameof(Controller)))
        {
        }

        protected View([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Draw(GameTime gameTime)
        {

        }

        void IView.SetOwner([NotNull] Scene ownedScene)
        {
            _ownedScene = ownedScene ?? throw new ArgumentNullException(nameof(ownedScene));
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
