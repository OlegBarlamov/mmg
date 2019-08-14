using System;
using FrameworkSDK.Game;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.Services.Graphics
{
    [UsedImplicitly]
    internal class DefaultSpriteBatchProvider : ISpriteBatchProvider
    {
        public SpriteBatch SpriteBatch => GameShell.SpriteBatch;

        [NotNull] private GameShell GameShell { get; }

        public DefaultSpriteBatchProvider([NotNull] GameShell gameShell)
        {
            GameShell = gameShell ?? throw new ArgumentNullException(nameof(gameShell));
        }
    }
}
