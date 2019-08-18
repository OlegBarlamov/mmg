using System;
using System.Diagnostics;
using FrameworkSDK;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Game.Views;
using FrameworkSDK.Services.Graphics;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace HeroesGame
{
    internal class TestApplication : Application
    {
        private TestScene _scene;

        public override Scene CurrentScene => GetCurrentScene();

        private Scene GetCurrentScene()
        {
            return _scene ?? (_scene = new TestScene());
        }
    }

    internal class TestScene : Scene
    {
        private readonly TestModel _model = new TestModel();

        public TestScene() : base("testScene")
        {
            AddController(_model);
        }
    }

    [UsedImplicitly]
    public class TestView : View<TestModel, TestController>
    {
        private ISpriteBatchProvider SpriteBatchProvider { get; }

	    private int _lastCounter;

        public TestView(ISpriteBatchProvider spriteBatchProvider)
        {
            SpriteBatchProvider = spriteBatchProvider ?? throw new ArgumentNullException(nameof(spriteBatchProvider));
        }

        public override void Draw(GameTime gameTime)
        {
            var counter = DataModel.Count;
			if (counter != _lastCounter)
				Console.WriteLine(counter);

	        _lastCounter = counter;
        }
    }

    public class TestController : Controller<TestModel>
    {
        private TimeSpan _timeSpent;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _timeSpent += gameTime.ElapsedGameTime;
            Model.Count = (int)_timeSpent.TotalSeconds;
        }
    }

    [UsedImplicitly]
    public class TestModel
    {
        public int Count { get; set; }
    }
}
