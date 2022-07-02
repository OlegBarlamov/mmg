using FrameworkSDK;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using MonoGameExtensions.DataStructures;
using X4World.Generation;

namespace X4World
{
    public interface IWrappedDetails : ILocatable3D, IGeneratorTarget, INamed
    {
        Vector3 GetWorldPosition();
        IWrappedDetails Parent { get; }
        
        object AggregatedData { get; }
        
        float DistanceToUnwrapDetails { get; }

        bool IsDetailsGenerated { get; }
        IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
    }
}