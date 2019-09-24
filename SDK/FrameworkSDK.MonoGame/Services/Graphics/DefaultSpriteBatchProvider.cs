using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Services.Graphics
{
    [UsedImplicitly]
    internal class DefaultSpriteBatchProvider : ISpriteBatchProvider
    {
        public SpriteBatch SpriteBatch => Game.SpriteBatch;

        [NotNull] private IGame Game { get; }

        public DefaultSpriteBatchProvider([NotNull] IGame game)
        {
	        Game = game ?? throw new ArgumentNullException(nameof(game));
        }
    }
}
