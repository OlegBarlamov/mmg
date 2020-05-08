using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using TablePlatform.Data.Tokens;

namespace TablePlatform.Data.Implementations
{
    public class Card : ICanvasCard
    {
        public Vector2 Position { get; }
        public float Rotation { get; }
        public float Width => CardType.MetaType.Width;
        public float Height => CardType.MetaType.Height;
        public float Left => Position.X;
        public float Right => Left + Width;
        public float Top => Position.Y;
        public float Bottom => Top + Height;
        public Point LeftTop => new Point((int)Left, (int)Top);
        
        public ICanvasCardType CardType { get; }
        
        private readonly IToken _token = new SimpleGuidToken(Guid.NewGuid());

        public Card([NotNull] ICanvasCardType cardType, Vector2 position)
        {
            CardType = cardType ?? throw new ArgumentNullException(nameof(cardType));
            Position = position;
        }
        
        public IToken GetToken()
        {
            return _token;
        }
    }
}