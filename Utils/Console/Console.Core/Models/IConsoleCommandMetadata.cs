using JetBrains.Annotations;

namespace Console.Core.Models
{
    public interface IConsoleCommandMetadata
    {
        [NotNull] string Title { get; }

        [NotNull] string Description { get; }
        
        [CanBeNull] object Data { get; }
    }
}