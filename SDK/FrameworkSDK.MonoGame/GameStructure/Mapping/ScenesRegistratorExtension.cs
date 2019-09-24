using System;
using FrameworkSDK.MonoGame.GameStructure.Scenes;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
    public static class ScenesRegistratorExtension
    {
        public static void RegisterScene<TModel, TScene>([NotNull] this IScenesRegistrator scenesRegistrator)
            where TScene : Scene
            where TModel : class
        {
            if (scenesRegistrator == null) throw new ArgumentNullException(nameof(scenesRegistrator));
            scenesRegistrator.RegisterScene(typeof(TModel), typeof(TScene));
        }
    }
}
