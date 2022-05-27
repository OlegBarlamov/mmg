using System;
using Atom.Client.MacOS.Components;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
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

        private BasicEffect _effect;
        
        public MainScene(MainSceneDataModel model)
            :base("MainScene")
        {
            DataModel = model;
            
            AddView(new RectangleModel(DataModel.ColorsTexturesPackage.Get(Color.Blue)));
            AddView(new Grid3DComponentView());
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _effect?.Dispose();
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            _effect = new BasicEffect(graphicsPipelineBuilder.GraphicsDevice);
            //Camera
            Matrix projection = Matrix.
                CreatePerspectiveFieldOfView(
                    (float)Math.PI / 4.0f, 
                    (float)4/3,
                    1f, 10f);
            _effect.Projection = projection;
            Matrix v = 
                Matrix.CreateLookAt(new Vector3(2, 2, 2), Vector3.Zero, Vector3.Up);
            _effect.View = v;
            _effect.World = Matrix.Identity;
            //// 
            
            _effect.VertexColorEnabled = true;

            var vertexBuffer = new VertexBuffer(graphicsPipelineBuilder.GraphicsDevice, VertexPositionColor.VertexDeclaration, 100, BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsPipelineBuilder.GraphicsDevice, typeof(int), 100, BufferUsage.WriteOnly);
            var action = new SimpleRenderComponentsMeshes<VertexPositionColor>("render", _effect, vertexBuffer, indexBuffer);
            
            return graphicsPipelineBuilder
                .Clear(Color.Black)
                .AddAction(action)
                // .Do((time, context, components) =>
                // {
                //     
                // }, "render")
                .Build();
        }
    }
}