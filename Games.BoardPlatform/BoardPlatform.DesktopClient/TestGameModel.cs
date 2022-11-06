using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TablePlatform.Data;
using TablePlatform.Data.Implementations;
using TablePlatform.Data.Tokens;

namespace TablePlatform.DesktopClient
{
    public class TestGameModel : ITableGameDescriptor
    {
        public IReadOnlyCollection<ICanvasCardMetaType> CardTypes { get; }
        public IReadOnlyCollection<ICanvasCard> InitialPosition { get; }

        public TestGameModel(GamePackage package)
        {
            var cardType = new CardMetaType(new SimpleGuidToken(Guid.Empty), new SimpleGuidToken(Guid.Empty));
            cardType.AvailableForwardTextures.Add(package.Texture.GetToken());
            CardTypes = new[]
            {
                cardType
            };

            InitialPosition = new[]
            {
                new Card(new CardType(cardType), Vector2.Zero),
            };
        }
    }
}