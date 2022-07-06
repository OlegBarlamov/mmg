using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Meshes
{
    public static class RenderableMeshExtensions
    {
        public static void SetYawPitchRoll(this IRenderableMesh mesh, float yaw, float pitch, float roll)
        {
            mesh.Rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
        }
        
        public static void SetYawPitchRoll(this IRenderableMesh mesh, Vector3 yawPitchRollVector)
        {
            mesh.SetYawPitchRoll(yawPitchRollVector.X, yawPitchRollVector.Y, yawPitchRollVector.Z);
        }
    }
}