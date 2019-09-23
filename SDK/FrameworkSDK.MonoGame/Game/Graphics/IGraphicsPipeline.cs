using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game.Graphics
{
    public interface IGraphicsPipeline
    {
        void Process([NotNull] GameTime gameTime, [NotNull, ItemNotNull] IReadOnlyCollection<IGraphicComponent> components);
    }
}
