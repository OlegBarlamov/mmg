using System;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Presets;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Map;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents;
using FrameworkSDK.MonoGame.SceneComponents.Controllers;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;
using Omegas.Client.MacOs.Models;
using Omegas.Client.MacOs.Models.SphereObject;
using SimplePhysics2D;
using SimplePhysics2D.Objects;
using SimplePhysics2D.Spaces;

namespace Omegas.Client.MacOs
{
    public class MainScene : Scene
    {
        public IInputService InputService { get; }
        public IRandomService RandomService { get; }
        public GameResourcePackage GameResourcePackage { get; }
        public ICamera2DService Camera2DService { get; }
        public IDisplayService DisplayService { get; }
        public ICollisionDetector2D CollisionDetector2D { get; }
        private PlayerData Player1Data { get; set; }
        private PlayerData Player2Data { get; set; }
        
        private Tiled2DMap _map;
        private TiledMapDrawableComponent _mapDrawableComponent;

        private SimpleCamera2D _player1Camera;
        private SimpleCamera2D _player2Camera;
        
        public MainScene(
            [NotNull] IInputService inputService,
            [NotNull] IRandomService randomService,
            [NotNull] GameResourcePackage gameResourcePackage,
            [NotNull] ICamera2DService camera2DService,
            [NotNull] IDisplayService displayService,
            [NotNull] IPhysics2DFactory physics2DFactory,
            [NotNull] ICollisionDetector2D collisionDetector2D)
            : base("MainScene")
        {
            if (physics2DFactory == null) throw new ArgumentNullException(nameof(physics2DFactory));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            GameResourcePackage = gameResourcePackage ?? throw new ArgumentNullException(nameof(gameResourcePackage));
            Camera2DService = camera2DService ?? throw new ArgumentNullException(nameof(camera2DService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            CollisionDetector2D = collisionDetector2D ?? throw new ArgumentNullException(nameof(collisionDetector2D));

            UsePhysics2D(physics2DFactory.Create(new HeapCollidersSpace2D()));
        }

        protected override void Initialize()
        {
            // Map
            _map = new OmegaTiledMap(GameResourcePackage.MapBackgroundTexturesList);
            _mapDrawableComponent = (TiledMapDrawableComponent) AddView(new ViewModel<Tiled2DMap>(_map));
            Physics2D.AddBody(new MapBoundaries(new RectangleF(0, 0, _map.WorldSize.X, _map.WorldSize.Y)));

            Player1Data = new PlayerData(PlayerIndex.One, Color.Red, Color.DarkRed, new Vector2(200, 200), SphereObjectData.GetHealthFromRadius(50f));
            Player2Data = new PlayerData(PlayerIndex.Two, Color.Blue, Color.DarkBlue, new Vector2(_map.WorldSize.X - 200, _map.WorldSize.Y - 200), SphereObjectData.GetHealthFromRadius(50f));
            
            for (int i = 0; i < 50; i++)
            {
                PlaceNeutral(4f, 10f);
            }
            for (int i = 0; i < 10; i++)
            {
                PlaceNeutral(10f, 30f);
            }
            for (int i = 0; i < 8; i++)
            {
                PlaceNeutral(30f, 60f);
            }
            for (int i = 0; i < 3; i++)
            {
                PlaceNeutral(60f, 120f);
            }

            // Players
            AddView(Player1Data);
            AddView(Player2Data);

            // Camera
            AddController(new CenteredCameraController(Player1Data, _player1Camera));
            AddController(new CenteredCameraController(Player2Data, _player2Camera));
            // AddController(new KeyboardObject2DPositioningController(InputService, Player1Data,
            //     new KeyboardPositioningController.KeysMap()));
            //Debug
            // AddView(new DebugInfoComponentData
            // {
            //     Font = GameResourcePackage.Font,
            //     Tab = 15f,
            // });
        }

        private void PlaceNeutral(float sizeMin, float sizeMax)
        {
            var size = RandomService.NextFloat(sizeMin, sizeMax);
            var health = SphereObjectData.GetHealthFromRadius(size);
            bool collide = true;
            Vector2 position = Vector2.Zero;
            SphereObjectData neutral = null;
            while (collide)
            {
                position = new Vector2(
                    RandomService.NextFloat(size, _map.WorldSize.X - size),
                    RandomService.NextFloat(size, _map.WorldSize.Y - size));
                
                neutral = new SphereObjectData(Color.LightGray, position, health, Teams.Neutral);
                var collision1 = CollisionDetector2D.GetCollision(neutral.Fixture, Player1Data.Fixture);
                var collision2 = CollisionDetector2D.GetCollision(neutral.Fixture, Player2Data.Fixture);
                collide = !collision1.IsEmpty || !collision2.IsEmpty;
            }
                
            AddView(neutral);
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder builder)
        {
            var displaySize = new SizeInt(
                DisplayService.PreferredBackBufferWidth,
                DisplayService.PreferredBackBufferHeight);
            
            var clearColor = Color.DarkMagenta;
            var player1RenderTarget = builder.RenderTargetsFactoryService.CreateRenderTarget(
                displaySize.Width / 2,
                displaySize.Height
            );
            var player2RenderTarget = builder.RenderTargetsFactoryService.CreateRenderTarget(
                displaySize.Width / 2,
                displaySize.Height
            );
            var player2RenderTargetOffset = new Vector2((float)displaySize.Width / 2, 0);
            
            _player1Camera = new SimpleCamera2D(new SizeInt(displaySize.Width / 2, displaySize.Height));
            _player2Camera = new SimpleCamera2D(new SizeInt(displaySize.Width / 2, displaySize.Height));
            
            var separatorRec = new RectangleF((float)displaySize.Width / 2, 0, 1, displaySize.Height);
            
            return builder
                .SetRenderTarget(player1RenderTarget)
                .Clear(clearColor)
                .SetActiveCamera2D(() => _player1Camera)
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents(GraphicsPipeline2DDrawingPreset.PipelineActions.DrawComponents)
                .DrawComponents(GraphicsPipeline2DDrawingPreset.PipelineActions.DrawDebugComponents)
                .EndDraw()
                .SetRenderTarget(player2RenderTarget)
                .Clear(clearColor)
                .SetActiveCamera2D(() => _player2Camera)
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents(GraphicsPipeline2DDrawingPreset.PipelineActions.DrawComponents)
                .DrawComponents(GraphicsPipeline2DDrawingPreset.PipelineActions.DrawDebugComponents)
                .EndDraw()
                .SetRenderTargetToDisplay()
                .Clear(clearColor)
                .BeginDraw(new BeginDrawConfig())
                .DrawRenderTarget(player1RenderTarget)
                .DrawRenderTarget(player2RenderTarget, () => player2RenderTargetOffset)
                .Do(context =>
                {
                    context.DrawContext.Draw(GameResourcePackage.SolidColor, separatorRec, Color.WhiteSmoke);
                })
                .DrawComponents(context => context.Camera2DService.GetScreenCamera(), GraphicsPipeline2DDrawingPreset.PipelineActions.DrawUI)
                .DrawComponents(context => context.Camera2DService.GetScreenCamera(), GraphicsPipeline2DDrawingPreset.PipelineActions.DrawDebugUI)
                .EndDraw()
                .Build();
            // return graphicsPipelineBuilder.Drawing2DPreset(Color.DarkMagenta)
            //     .Build();
        }
    }
}