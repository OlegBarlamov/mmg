using System;
using System.Diagnostics;
using FrameworkSDK;
using FrameworkSDK.Common.Services.Graphics;
using FrameworkSDK.Constructing;
using FrameworkSDK.Game;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroesGame
{
    internal class TestApplication : Application
    {
        private TestScene _scene;

        protected override Scene GetCurrentScene()
        {
            return _scene ?? (_scene = new TestScene());
        }

        protected override void Construct(IAppConstructor constructor)
        {
            
        }
    }

    internal class TestScene : Scene
    {
        private readonly TestModel _model = new TestModel();

        public TestScene() : base("testScene")
        {
            AddControllerByModel(_model);
        }
    }

    [UsedImplicitly]
    public class TestView : View<TestModel, TestController>
    {
        private ISpriteBatchProvider SpriteBatchProvider { get; }

        public TestView(ISpriteBatchProvider spriteBatchProvider)
        {
            SpriteBatchProvider = spriteBatchProvider ?? throw new ArgumentNullException(nameof(spriteBatchProvider));
        }

        public override void Draw(GameTime gameTime)
        {
            var counter = DataModel.Count;
            Trace.WriteLine(counter);
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
