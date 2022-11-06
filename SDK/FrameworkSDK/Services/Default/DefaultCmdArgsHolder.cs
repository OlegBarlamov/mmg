using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Services.Default
{
    public class DefaultCmdArgsHolder : ICmdArgsHolder
    {
        public IReadOnlyList<string> Args { get; }

        public DefaultCmdArgsHolder([NotNull] string[] args)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));
        }
    }
}