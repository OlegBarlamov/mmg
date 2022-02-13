using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Services
{
    public interface ICmdArgsHolder
    {
        [NotNull] IReadOnlyList<string> Args { get; }
    }
}