using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK
{
    public interface INamed
    {
        [NotNull] string Name { get; }
    }
}
