using System;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Presets;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Map;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Physics;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;
using Omegas.Client.MacOs.Controllers;
using Omegas.Client.MacOs.Models;
using Omegas.Client.MacOs.Models.SphereObject;
using Omegas.Client.MacOs.Services;
using SimplePhysics2D;
using SimplePhysics2D.Spaces;

namespace Omegas.Client.MacOs
{
    public class MainScene : Scene
    {
        private IRandomService RandomService { get; }
        private GameResourcePackage GameResourcePackage { get; }
        private IDisplayService DisplayService { get; }
        private OmegaGameService OmegaGameService { get; }
        private MapObjectsGenerator MapObjectsGenerator { get; }
        private PlayerData Player1Data { get; set; }
        private PlayerData Player2Data { get; set; }

        private OmegaTiledMap _map;
        private SimpleCamera2D _player1Camera;
        private SimpleCamera2D _player2Camera;

        private readonly IScene2DPhysicsSystem _physics;
        
        public MainScene(
            [NotNull] IRandomService randomService,
            [NotNull] GameResourcePackage gameResourcePackage,
            [NotNull] IDisplayService displayService,
            [NotNull] IPhysics2DFactory physics2DFactory,
            [NotNull] OmegaGameService omegaGameService,
            [NotNull] MapObjectsGenerator mapObjectsGenerator)
            : base("MainScene")
        {
            if (physics2DFactory == null) throw new ArgumentNullException(nameof(physics2DFactory));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            GameResourcePackage = gameResourcePackage ?? throw new ArgumentNullException(nameof(gameResourcePackage));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            OmegaGameService = omegaGameService ?? throw new ArgumentNullException(nameof(omegaGameService));
            MapObjectsGenerator = mapObjectsGenerator ?? throw new ArgumentNullException(nameof(mapObjectsGenerator));

            _physics = physics2DFactory.Create(new HeapCollidersSpace2D());
            this.UsePhysics2D(_physics);
        }

        protected override void Initialize()
        {
            OmegaGameService.Initialize(this, _physics);
            
            // Map
            _map = new OmegaTiledMap(GameResourcePackage);
            AddView(new ViewModel<Tiled2DMap>(_map));
            _physics.AddBody(new MapBoundaries(new RectangleF(0, 0, _map.WorldSize.X, _map.WorldSize.Y)));

            // Players
            Player1Data = MapObjectsGenerator.PlacePlayer1(_map.WorldSize.X, _map.WorldSize.Y);
            Player2Data = MapObjectsGenerator.PlacePlayer2(_map.WorldSize.X, _map.WorldSize.Y);

            // Objects
            MapObjectsGenerator.PlaceInitialObjects(_map.WorldSize.X, _map.WorldSize.Y, Player1Data, Player2Data);

            // Camera
            AddController(new OmegaCameraController(Player1Data, _player1Camera));
            AddController(new OmegaCameraController(Player2Data, _player2Camera));
            
            
            //Debug
            // AddView(new DebugInfoComponentData
            // {
            //     Font = GameResourcePackage.Font,
            //     Tab = 15f,
            // });
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder builder)
        {
            var displaySize = new SizeInt(
                DisplayService.PreferredBackBufferWidth,
                DisplayService.PreferredBackBufferHeight);
            
            var clearColor = Color.DarkMagenta;
            var player1RenderTarget = builder.RenderTargetsFactoryService.CreateDisplaySizedRenderTarget(
                size => new SizeInt(
                    size.Width / 2,
                    size.Height)
            );
            var player2RenderTarget = builder.RenderTargetsFactoryService.CreateDisplaySizedRenderTarget(
                size => new SizeInt(
                    size.Width / 2,
                    size.Height)
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