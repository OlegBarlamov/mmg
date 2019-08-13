using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.Common.Services.Graphics
{
    internal class DefaultSpriteBatchProvider : ISpriteBatchProvider
    {
        public SpriteBatch SpriteBatch => SpriteBatchFunc();

        [NotNull] private Func<SpriteBatch> SpriteBatchFunc { get; }

        public DefaultSpriteBatchProvider([NotNull] Func<SpriteBatch> spriteBatchFunc)
        {
            SpriteBatchFunc = spriteBatchFunc ?? throw new ArgumentNullException(nameof(spriteBatchFunc));
        }
    }
}
