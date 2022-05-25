using System;
using Atom.Client.MacOS.Components;
using Console.FrameworkAdapter;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.MacOS
{
    public class X4GameApp : GameApp
    {
        protected override SceneBase CurrentScene => _currentScene;
        
        private DefaultConsoleManipulator DefaultConsoleManipulator { get; }
        private ColorsTexturesPackage ColorsTexturesPackage { get; }

        private SceneBase _mainScene;
        private SceneBase _currentScene;

        public X4GameApp([NotNull] DefaultConsoleManipulator defaultConsoleManipulator, [NotNull] ColorsTexturesPackage colorsTexturesPackage)
        {
            DefaultConsoleManipulator = defaultConsoleManipulator ?? throw new ArgumentNullException(nameof(defaultConsoleManipulator));
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));
        }

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            _mainScene = new MainScene();
            _mainScene.AddController(DefaultConsoleManipulator);
            _currentScene = _mainScene;
            
            var rectangleData = new RectangleModel {Texture = ColorsTexturesPackage.Get(Color.Red)};
            _mainScene.AddView(rectangleData);
        }

    }
}