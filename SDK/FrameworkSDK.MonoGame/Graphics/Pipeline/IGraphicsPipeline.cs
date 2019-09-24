using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    public interface IGraphicsPipeline
    {
        void Process([NotNull] GameTime gameTime, [NotNull, ItemNotNull] IReadOnlyCollection<IGraphicComponent> components);
    }
}
