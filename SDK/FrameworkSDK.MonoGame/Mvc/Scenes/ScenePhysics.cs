using System;
using FrameworkSDK.MonoGame.Physics;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class ScenePhysics : SceneBase
    {
        [NotNull]
        public IScene2DPhysics Physics2D
        {
            get
            {
                if (_physics2D == null)
                    throw new FrameworkMonoGameException($"No physics engine assigned. Assign physic engine using {nameof(UsePhysics2D)} method.");

                return _physics2D;
            }
        }

        private bool IsPhysics2DUsed => _physics2D != null;

        [CanBeNull] private IScene2DPhysicsInternal _physics2D;
        
        protected ScenePhysics([NotNull] string name, object model = null)
            : base(name, model)
        {
        }
        
        protected void UsePhysics2D([NotNull] IScene2DPhysicsInternal physics)
        {
            _physics2D = physics ?? throw new ArgumentNullException(nameof(physics));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            _physics2D?.Update(gameTime);
        }

        protected override void OnViewAttached(IView view)
        {
            if (IsPhysics2DUsed && view.DataModel is IPhysicsBody2D physicsBody)
            {
                Physics2D.AddBody(physicsBody);
            }
            
            base.OnViewAttached(view);
        }

        protected override void OnViewDetached(IView view)
        {
            if (IsPhysics2DUsed && view.DataModel is IPhysicsBody2D physicsBody)
            {
                Physics2D.RemoveBody(physicsBody);
            }
            
            base.OnViewDetached(view);
        }
    }
}