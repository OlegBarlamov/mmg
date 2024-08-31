using System;
using FrameworkSDK.MonoGame.Physics;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public static class SceneExtensions
    {
        public static void UsePhysics2D([NotNull] this SceneBase scene, [NotNull] IScene2DPhysicsSystem physicsSystem)
        {
            if (scene == null) throw new ArgumentNullException(nameof(scene));
            if (physicsSystem == null) throw new ArgumentNullException(nameof(physicsSystem));
            
            scene.AddExtension(new ScenePhysics(physicsSystem));
        }
    }
}