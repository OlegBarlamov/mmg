using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using TablePlatform.Data;
using TablePlatform.Data.Tokens;

namespace TablePlatform.Client
{
    public class TextureUnifiedWrapper : IUnified
    {
        public Texture2D Texture2D { get; }
        
        private readonly IToken _token = new SimpleGuidToken(Guid.NewGuid());

        public TextureUnifiedWrapper([NotNull] Texture2D texture2D)
        {
            Texture2D = texture2D ?? throw new ArgumentNullException(nameof(texture2D));
        }
        
        public IToken GetToken()
        {
            return _token;
        }
    }
}