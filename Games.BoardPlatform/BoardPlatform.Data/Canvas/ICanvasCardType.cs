using JetBrains.Annotations;

namespace TablePlatform.Data
{
    public interface ICanvasCardType : IUnified
    {
        ICanvasCardMetaType MetaType { get; }
        IToken ForwardTexture { get; }
    }
}