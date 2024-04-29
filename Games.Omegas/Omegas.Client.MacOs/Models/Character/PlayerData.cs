using FrameworkSDK.MonoGame.Physics._2D.Forces;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using Omegas.Client.MacOs.Models.SphereObject;
using SimplePhysics2D.Fixtures;

namespace Omegas.Client.MacOs.Models
{
    public class PlayerData : SphereObjectData
    {
        public Vector2 HeartRelativePosition { get; private set; } = Vector2.Zero;

        public float HeartSize = 10;

        public Color HeartColor { get; } = Color.DarkRed;

        public SimpleForce ControllerForce { get; } = new SimpleForce(Vector2.Zero, 0f);

        public PlayerIndex PlayerIndex { get; }
        
        public override void SetPosition(Vector2 position)
        {
            base.SetPosition(position);
            PlayerViewModel.HeartBoundingBox = GetHeartBoundingBox();
        }

        public void SetHeartRelativePosition(Vector2 relativePosition)
        {
            HeartRelativePosition = relativePosition;
            PlayerViewModel.HeartBoundingBox = GetHeartBoundingBox();
        }

        public override SphereObjectViewModel ViewModel => PlayerViewModel;
        
        public PlayerViewModel PlayerViewModel { get; }

        public PlayerData(PlayerIndex playerIndex, Color color, Vector2 position, float size)
            : base(color, position, size)
        {
            PlayerIndex = playerIndex;
            PlayerViewModel  = new PlayerViewModel
            {
                BoundingBox = GetBoundingBox(),
                HeartBoundingBox = GetHeartBoundingBox(),
                Color = Color,
                HeartColor = HeartColor
            };
            ActiveForces.Add(ControllerForce);
        }

        private RectangleF GetHeartBoundingBox()
        {
            return RectangleF.FromCenterAndSize(
                Position + HeartRelativePosition,
                HeartSize);
        }
    }
}