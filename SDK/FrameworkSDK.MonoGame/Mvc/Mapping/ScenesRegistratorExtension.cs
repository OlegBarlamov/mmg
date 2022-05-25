using System;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public static class ScenesRegistratorExtension
    {
        public static void RegisterScene<TModel, TScene>([NotNull] this IScenesRegistrator scenesRegistrator)
            where TScene : SceneBase
            where TModel : class
        {
            if (scenesRegistrator == null) throw new ArgumentNullException(nameof(scenesRegistrator));
            scenesRegistrator.RegisterScene(typeof(TModel), typeof(TScene));
        }
    }
}
