using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Mvc;
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
            var viewsArray = views.ToArray();

            foreach (var component in viewsArray)
            {
                component.Render(gameTime, Context.RenderContext);
            }

            Context.GraphicDevice.BeginDraw();
            try
            {
                foreach (var component in viewsArray)
                {
                    component.Draw(gameTime, Context.DrawContext);
                }
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
