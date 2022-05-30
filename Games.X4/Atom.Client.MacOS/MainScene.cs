using System;
using Console.FrameworkAdapter;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.SceneComponents;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.MacOS
{
    [UsedImplicitly]
    public class MainSceneDataModel
    {
        public ColorsTexturesPackage ColorsTexturesPackage { get; }

        public MainSceneDataModel([NotNull] ColorsTexturesPackage colorsTexturesPackage)
        {
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));
        }
    }
    
    public class MainScene : Scene
    {
        private MainSceneDataModel DataModel { get; }
        public ICamera3DService Camera3DService { get; }

        private BasicEffect _effect;
        private readonly DirectionalCamera3D _camera = new DirectionalCamera3D(new Vector3(10, 10, 10), Vector3.Zero);
        private readonly FirstPersonCameraController _cameraController;

        private IView _boxView;
        
        public MainScene(MainSceneDataModel model, [NotNull] ICamera3DService camera3DService, IInputService inputService, IConsoleResourcePackage consoleResourcePackage)
            :base("MainScene")
        {
            DataModel = model;
            Camera3DService = camera3DService ?? throw new ArgumentNullException(nameof(camera3DService));

            Camera3DService.SetActiveCamera(_camera);
            
            var gridData = new Grid3DComponentData
            {
                GraphicsPassName = "Render_Identical"
            };
            AddView(gridData);

            for (int i = 0; i < 50; i++)
            {
                var boxData = new BoxComponentDataModel
                {
                    GraphicsPassName = "Render_Identical",
                    Color = Color.Pink,
                    //Scale = new Vector3(2),
                    Position = new Vector3(i*3f,(int)((i / 5f)*2), 0),
                };
                _boxView = AddView(boxData);
            }

            AddView(new DebugInfoComponentData
            {
                Font = consoleResourcePackage.ConsoleFont,
                FontColor = Color.White,
                Position = new Vector2(10f),
                Tab = 20f,
            });

            _cameraController = new FirstPersonCameraController(inputService, _camera);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            _cameraController.Update(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _effect?.Dispose();
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            _effect = new BasicEffect(graphicsPipelineBuilder.GraphicsDevice);
            _effect.VertexColorEnabled = true;

            var vertexBuffer = new VertexBuffer(graphicsPipelineBuilder.GraphicsDevice, VertexPositionColor.VertexDeclaration, 100, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsPipelineBuilder.GraphicsDevice, typeof(int), 200, BufferUsage.WriteOnly);
            
            var vertexBuffer2 = new VertexBuffer(graphicsPipelineBuilder.GraphicsDevice, VertexPositionColor.VertexDeclaration, 100, BufferUsage.WriteOnly);
            var indexBuffer2 = new IndexBuffer(graphicsPipelineBuilder.GraphicsDevice, typeof(int), 200, BufferUsage.WriteOnly);

            return graphicsPipelineBuilder
                .Clear(Color.Black)
                .SetActiveCamera(_effect)
                .SimpleRender<VertexPositionColor>(_effect, vertexBuffer, indexBuffer, "Render")
                .RenderIdentical<VertexPositionColor>(_effect, vertexBuffer2, indexBuffer2,  "Render_Identical")
                .BeginDraw(new BeginDrawConfig())
                .DrawComponents()
                .EndDraw()
                .Build();
        }
    }
}