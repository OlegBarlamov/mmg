using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Core
{
    internal interface IGameHostProvider
    {
        [NotNull] IGameHost GameHost { get; }
    }
}