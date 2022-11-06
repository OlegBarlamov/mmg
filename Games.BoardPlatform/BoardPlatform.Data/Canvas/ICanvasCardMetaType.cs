using System.Collections.Generic;
using JetBrains.Annotations;

namespace TablePlatform.Data
{
    public interface ICanvasCardMetaType : IUnified, ISizable
    {
        IReadOnlyList<IToken> AvailableForwardTextures { get; }
        [CanBeNull] IToken ForwardTextureBackground { get; }
        IToken BackwardTexture { get; }
    }
}