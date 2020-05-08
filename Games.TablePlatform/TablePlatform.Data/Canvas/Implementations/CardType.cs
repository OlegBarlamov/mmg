using System;
using System.Linq;
using JetBrains.Annotations;
using TablePlatform.Data.Tokens;

namespace TablePlatform.Data.Implementations
{
    public class CardType : ICanvasCardType
    {
        public ICanvasCardMetaType MetaType { get; }
        public IToken ForwardTexture { get; }
        
        private readonly IToken _token = new SimpleGuidToken(Guid.NewGuid());

        public CardType([NotNull] ICanvasCardMetaType metaType)
        {
            MetaType = metaType ?? throw new ArgumentNullException(nameof(metaType));

            ForwardTexture = metaType.AvailableForwardTextures.First();
        }
        
        public IToken GetToken()
        {
            return _token;
        }
    }
}