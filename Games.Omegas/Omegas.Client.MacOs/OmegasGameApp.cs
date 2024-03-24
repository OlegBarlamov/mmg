using System;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace Template.MacOs
{
    public class OmegasGameApp : GameApp
    {
        public IInputService InputService { get; }

        public OmegasGameApp([NotNull] IInputService inputService)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            CurrentScene = new MainScene(InputService);
        }
        protected override SceneBase CurrentScene { get; }
    }
}