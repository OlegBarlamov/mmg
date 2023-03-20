using System;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace River.Client.MacOS
{
    internal class RiverGameApp : GameApp
    {
        protected override SceneBase CurrentScene => _currentScene;

        private SceneBase _currentScene = new EmptyScene();
        
        private MainScene MainScene { get; }

        public RiverGameApp([NotNull] MainScene mainScene)
        {
            MainScene = mainScene ?? throw new ArgumentNullException(nameof(mainScene));
        }

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();

            _currentScene = MainScene;
        }
    }
}