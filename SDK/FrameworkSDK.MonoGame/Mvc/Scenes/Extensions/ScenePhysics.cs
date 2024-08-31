using System;
using FrameworkSDK.MonoGame.Physics;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Mvc
{
    public class ScenePhysics : ISceneExtension
    {
        [NotNull] public IScene2DPhysics Physics2D => _physics2D;

        public string Name => nameof(ScenePhysics);
        
        private readonly IScene2DPhysicsSystem _physics2D;
        
        public ScenePhysics([NotNull] IScene2DPhysicsSystem physics)
        {
            _physics2D = physics ?? throw new ArgumentNullException(nameof(physics));
        }

        public void Update(GameTime gameTime)
        {
            _physics2D.Update(gameTime);
        }

        public void OnClosed()
        {
        }

        public void OnOpened()
        {
        }

        public void OnOpening()
        {
        }

        public void OnViewAttached(IView view)
        {
            if (view.DataModel is IPhysicsBody2D physicsBody)
            {
                Physics2D.AddBody(physicsBody);
            }
        }

        public void OnViewDetached(IView view)
        {
            if (view.DataModel is IPhysicsBody2D physicsBody)
            {
                Physics2D.RemoveBody(physicsBody);
            }
        }

        public void OnControllerAttached(IController controller)
        {
        }

        public void OnControllerDetached(IController controller)
        {
        }
    }
}