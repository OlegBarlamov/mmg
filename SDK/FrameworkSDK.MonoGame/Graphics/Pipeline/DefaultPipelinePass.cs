using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    internal class DefaultPipelinePass : IGraphicsPass
    {
        public string Name { get; } = "default";

        private IGraphicsPipelineContext Context { get; set; }

        public void Prepare(IGraphicsPipelineContext context)
        {
            Context = context;
        }

        public void Process(GameTime gameTime, IEnumerable<IGraphicComponent> views)
        {
            foreach (var component in views)
            {
                component.Render(gameTime, Context.RenderContext);
            }

            Context.GraphicDevice.BeginDraw();
            try
            {

            }
            catch (Exception e)
            {
                //TODO
            }
            finally 
            {
                Context.GraphicDevice.EndDraw();
            }
        }

        public void End()
        {
        }
    }
}
