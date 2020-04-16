using JetBrains.Annotations;

namespace Console.Core.Models
{
    public interface IConsoleCommand
    {
        [NotNull] string Text { get; }
        
        [CanBeNull] IConsoleCommandMetadata Metadata { get; }
    }
}