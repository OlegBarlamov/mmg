using System;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
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
        private PlayerData Player1Data { get; } = new PlayerData(PlayerIndex.One, Color.Red, new Vector2(200, 200), 50f);
        private PlayerData Player2Data { get; } = new PlayerData(PlayerIndex.Two, Color.Blue, new Vector2(800, 200), 50f);
        
        private Tiled2DMap _map;
        private TiledMapDrawableComponent _mapDrawableComponent;
        
        public MainScene([NotNull] IInputService inputService, [NotNull] IRandomService randomService,
            [NotNull] GameResourcePackage gameResourcePackage, [NotNull] ICamera2DService camera2DService,
            [NotNull] IDisplayService displayService, [NotNull] IPhysics2DFactory physics2DFactory)
            : base("MainScene")
        {
            if (physics2DFactory == null) throw new ArgumentNullException(nameof(physics2DFactory));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            GameResourcePackage = gameResourcePackage ?? throw new ArgumentNullException(nameof(gameResourcePackage));
            Camera2DService = camera2DService ?? throw new ArgumentNullException(nameof(camera2DService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            
            UsePhysics2D(physics2DFactory.Create(new HeapCollidersSpace2D()));
        }

        protected override void Initialize()
        {
            // Map
            _map = new OmegaTiledMap(GameResourcePackage.MapBackgroundTexturesList);
            _mapDrawableComponent = (TiledMapDrawableComponent) AddView(new ViewModel<Tiled2DMap>(_map));
            Physics2D.AddBody(new PhysicsMapBounds2D(new RectangleF(0, 0, _map.WorldSize.X, _map.WorldSize.Y)));
            
            // Players
            AddView(Player1Data);
            AddView(Player2Data);

            // Camera
            var camera = new SimpleCamera2D(new SizeInt(DisplayService.PreferredBackBufferWidth,
                DisplayService.PreferredBackBufferHeight));
            Camera2DService.SetActiveCamera(camera);
            AddController(new CenteredCameraController(Player1Data, camera));
            
            // Debug
            // AddView(new DebugInfoComponentData
            // {
            //     Font = GameResourcePackage.Font,
            // });

            // Test
            // Physics2D.ApplyImpulse(Player2Data, new Vector2(-20f, 0));
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset(Color.CornflowerBlue, new BeginDrawConfig()).Build();
        }
    }
}