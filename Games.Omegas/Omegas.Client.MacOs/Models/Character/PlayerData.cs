using FrameworkSDK.MonoGame.Physics._2D.Forces;
using FrameworkSDK.MonoGame.Physics2D;
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

        public PlayerIndex PlayerIndex { get; }
        
        public Vector2 CameraOffsetDirection { get; set; }

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

        public PlayerViewModel PlayerViewModel { get; }

        public PlayerData(PlayerIndex playerIndex, Color color, Color heartColor, Vector2 position, float health)
            : base(color, position, health, playerIndex.ToTeam())
        {
            PlayerIndex = playerIndex;
            HeartColor = heartColor;
            PlayerViewModel  = new PlayerViewModel(ViewModel)
            {
                HeartBoundingBox = GetHeartBoundingBox(),
                HeartColor = HeartColor
            };
        }

        private RectangleF GetHeartBoundingBox()
        {
            return RectangleF.FromCenterAndSize(
                Position + HeartRelativePosition,
                HeartSize);
        }
    }
}