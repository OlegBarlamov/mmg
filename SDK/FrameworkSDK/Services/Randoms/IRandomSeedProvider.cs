using System;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.Common
{
    public interface IRandomSeedProvider
    {
        Random Seed { get; }
    }
}
