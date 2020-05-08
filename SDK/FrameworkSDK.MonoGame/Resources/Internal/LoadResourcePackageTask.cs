using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    internal class LoadResourcePackageTask : IEquatable<LoadResourcePackageTask>
    {
        public IResourcePackage Package { get; }
        public bool UseBackgroundThread { get; }

        public LoadResourcePackageTask([NotNull] IResourcePackage package, bool useBackgroundThread)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            UseBackgroundThread = useBackgroundThread;
        }

        public bool Equals(LoadResourcePackageTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Package, other.Package) && UseBackgroundThread == other.UseBackgroundThread;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LoadResourcePackageTask) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Package != null ? Package.GetHashCode() : 0) * 397) ^ UseBackgroundThread.GetHashCode();
            }
        }
    }
}