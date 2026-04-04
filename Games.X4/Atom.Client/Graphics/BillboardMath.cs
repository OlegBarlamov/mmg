using Microsoft.Xna.Framework;

namespace Atom.Client.Graphics
{
    public static class BillboardMath
    {
        public static Matrix GetBillboardRotation(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUp)
        {
            var normal = cameraPosition - objectPosition;
            normal.Normalize();
            if (normal == Vector3.Up)
                return Matrix.Identity;
            if (normal == Vector3.Down)
                return Matrix.CreateRotationX(MathHelper.Pi);

            var rotationAxis = Vector3.Cross(cameraUp, normal);
            rotationAxis.Normalize();
            var down = Vector3.Cross(normal, rotationAxis);
            return Matrix.CreateRotationX(3 * MathHelper.Pi / 2) * new Matrix
            {
                M11 = -rotationAxis.X,
                M12 = -rotationAxis.Y,
                M13 = -rotationAxis.Z,
                M14 = 0.0f,
                M21 = down.X,
                M22 = down.Y,
                M23 = down.Z,
                M24 = 0.0f,
                M31 = normal.X,
                M32 = normal.Y,
                M33 = normal.Z,
                M34 = 0.0f,
                M41 = 0f,
                M42 = 0f,
                M43 = 0f,
                M44 = 1f
            };
        }
    }
}
