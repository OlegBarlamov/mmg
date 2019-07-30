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

        object IView.DataModel => _dataModel;

        IController IView.Controller => _controller;

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

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {

        }

        protected virtual void SetDataModel([NotNull] object dataModel)
        {
            _dataModel = dataModel ?? throw new ArgumentNullException(nameof(dataModel));
        }

        protected virtual void SetController([NotNull] IController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        void IView.SetOwner([NotNull] Scene ownedScene)
        {
            _ownedScene = ownedScene ?? throw new ArgumentNullException(nameof(ownedScene));
        }

        void IView.SetDataModel(object dataModel)
        {
            if (_dataModel == null)
                SetDataModel(dataModel);
        }

        void IView.SetController(IController controller)
        {
            if (_controller == null)
                SetController(controller);
        }
    }
}
