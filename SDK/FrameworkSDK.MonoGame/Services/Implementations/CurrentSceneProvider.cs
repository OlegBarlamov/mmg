using System;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    [UsedImplicitly]
    internal class CurrentSceneProvider : ICurrentSceneProvider
    {
        public SceneBase CurrentScene => (SceneBase)ScenesController.CurrentScene;

        private IScenesController ScenesController { get; }
        
        public CurrentSceneProvider([NotNull] IScenesController scenesController)
        {
            ScenesController = scenesController ?? throw new ArgumentNullException(nameof(scenesController));
        }
    }
}