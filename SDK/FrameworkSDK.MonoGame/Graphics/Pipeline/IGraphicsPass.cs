using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    public interface IGraphicsPass : INamed
    {
        void Prepare(IGraphicsPipelineContext context);

        void Process(GameTime gameTime, IEnumerable<IGraphicComponent> views);

        void End();
    }
}
