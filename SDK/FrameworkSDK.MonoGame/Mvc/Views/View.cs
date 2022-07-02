using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class View : IView
    {
        public const string DefaultViewPassName = "Default";

        private static readonly IReadOnlyList<string> DefaultViewPassNames = new[] {DefaultViewPassName};  

        public string Name { get; protected set; }

        SceneBase ISceneComponent.OwnedScene => _ownedScene;

        object IView.DataModel => _dataModel;

        IController IView.Controller => _controller;

        private object _dataModel;
        private IController _controller;
        private SceneBase _ownedScene;

        private readonly List<IView> _children = new List<IView>();

        protected View()
        {
            Name = NamesGenerator.Hash(HashType.SmallGuid, GetType().Name);
        }

        public void Destroy()
        {
            // TODO implement destroy
            OnDestroy();
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual IReadOnlyList<string> GraphicsPassNames => DefaultViewPassNames;
        public virtual BoundingBox? BoundingBox { get; protected set; } = null;

        public virtual IReadOnlyDictionary<string, IReadOnlyList<IRenderableMesh>> MeshesByPass { get; } = new Dictionary<string, IReadOnlyList<IRenderableMesh>>();

        public virtual void Draw(GameTime gameTime, IDrawContext context)
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

        protected virtual void OnAttached([NotNull] SceneBase scene)
        {

        }

        protected virtual void OnDetached([NotNull] SceneBase scene)
        {

        }

        protected virtual void OnDestroy()
        {
            
        }

        protected void AddChild([NotNull] IView childView)
        {
            if (childView == null) throw new ArgumentNullException(nameof(childView));

            var scene = _ownedScene;
            if (scene == null)
                throw new ScenesException(Strings.Exceptions.Scenes.SceneComponentNotAttached, this);

            scene.AddView(childView);
            _children.Add(childView);
        }

        [CanBeNull]
        protected IView AddChild([NotNull] object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var scene = _ownedScene;
            if (scene == null)
                throw new ScenesException(Strings.Exceptions.Scenes.SceneComponentNotAttached, this);

            var view = scene.AddView(model);
            _children.Add(view);
            return view;
        }

        protected void RemoveChild([NotNull] IView childView)
        {
            if (childView == null) throw new ArgumentNullException(nameof(childView));

            var scene = _ownedScene;
            if (scene == null)
                throw new ScenesException(Strings.Exceptions.Scenes.SceneComponentNotAttached, this);

            if (!_children.Contains(childView))
                throw new ScenesException(Strings.Exceptions.Scenes.ChildComponentNotExists, this, childView);

            RemoveChildInternal(childView);
        }

        private void RemoveChildInternal(IView childView)
        {
            _ownedScene.RemoveView(childView);
            _children.Remove(childView);
        }

        void ISceneComponent.OnAddedToScene(SceneBase scene)
        {
            _ownedScene = scene ?? throw new ArgumentNullException(nameof(scene));

            OnAttached(_ownedScene);
        }

        void ISceneComponent.OnRemovedFromScene(SceneBase scene)
        {
            while (_children.Count > 0)
            {
                RemoveChildInternal(_children[0]);
            }

            _ownedScene = null;
            OnDetached(scene);
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
