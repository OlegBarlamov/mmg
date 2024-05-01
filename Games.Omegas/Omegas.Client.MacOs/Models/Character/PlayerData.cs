using FrameworkSDK.MonoGame.Physics._2D.Forces;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using Omegas.Client.MacOs.Models.SphereObject;

namespace Omegas.Client.MacOs.Models
{
    public class PlayerData : SphereObjectData
    {
        public Vector2 HeartRelativePosition { get; private set; } = Vector2.Zero;

        public float HeartSize = 10;

        public Color HeartColor { get; }

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

        public PlayerData(PlayerIndex playerIndex, Color color, Color heartColor, Vector2 position, float size)
            : base(color, position, size, playerIndex.ToTeam())
        {
            PlayerIndex = playerIndex;
            HeartColor = heartColor;
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