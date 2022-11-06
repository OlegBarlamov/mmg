using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TablePlatform.Data.Tokens;

namespace TablePlatform.Data.Implementations
{
    public class CardMetaType : ICanvasCardMetaType
    {
        public float Width { get; set; } = 100;
        public float Height { get; set; } = 300;
        
        public IToken ForwardTextureBackground { get; }
        public IToken BackwardTexture { get; }

        public List<IToken> AvailableForwardTextures { get; } = new List<IToken>();

        IReadOnlyList<IToken> ICanvasCardMetaType.AvailableForwardTextures => AvailableForwardTextures;
        
        private readonly IToken _token = new SimpleGuidToken(Guid.NewGuid()); 
        
        public CardMetaType([NotNull] IToken forwardTextureBackground, [NotNull] IToken backwardTexture)
        {
            ForwardTextureBackground = forwardTextureBackground ?? throw new ArgumentNullException(nameof(forwardTextureBackground));
            BackwardTexture = backwardTexture ?? throw new ArgumentNullException(nameof(backwardTexture));
        }
        
        public IToken GetToken()
        {
            return _token;
        }
    }
}