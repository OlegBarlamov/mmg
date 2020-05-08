using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    internal class ResourceRecord : IEquatable<ResourceRecord>
    {
        public string AssetName { get; }
        public Type AssetType { get; }

        public override string ToString()
        {
            return $"Resource<{AssetType.Name}>:{AssetName}";
        }

        private ResourceRecord([NotNull] string assetName, [NotNull] Type assetType)
        {
            AssetName = assetName ?? throw new ArgumentNullException(nameof(assetName));
            AssetType = assetType ?? throw new ArgumentNullException(nameof(assetType));
        }

        public static ResourceRecord Create<T>(string assetName)
        {
            return new ResourceRecord(assetName, typeof(T));
        }
        
        public bool Equals(ResourceRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AssetName == other.AssetName && Equals(AssetType, other.AssetType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResourceRecord) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AssetName != null ? AssetName.GetHashCode() : 0) * 397) ^ (AssetType != null ? AssetType.GetHashCode() : 0);
            }
        }
    }
}