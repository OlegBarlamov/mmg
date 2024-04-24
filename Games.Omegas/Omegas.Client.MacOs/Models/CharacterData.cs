using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics._2D.BodyTypes;
using FrameworkSDK.MonoGame.Physics2D;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using SimplePhysics2D.Fixtures;

namespace Omegas.Client.MacOs.Models
{
    public class CharacterData : IColliderBody2D
    {
        public Vector2 Position { get; private set; } = new Vector2(50, 50);

        public Vector2 HeartRelativePosition { get; private set; } = Vector2.Zero;

        public float HeartSize = 10;

        public Color Color { get; } = Color.Red;

        public Color HeartColor { get; } = Color.DarkRed;

        public float Size { get; set; } = 50;

        public PlayerIndex PlayerIndex { get; set; } = PlayerIndex.One;
        
        public void SetPosition(Vector2 position)
        {
            Position = position;
            ViewModel.BoundingBox = GetBoundingBox();
            ViewModel.HeartBoundingBox = GetHeartBoundingBox();
            _fixture.Center = Position;
        }

        public void SetHeartRelativePosition(Vector2 relativePosition)
        {
            HeartRelativePosition = relativePosition;
            ViewModel.HeartBoundingBox = GetHeartBoundingBox();
        }

        public float Rotation { get; private set; }
        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public IPhysicsBody2DParameters Parameters { get; } = new DynamicBody2DParameters();
        public IScene2DPhysics Scene { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public ICollection<IForce2D> ActiveForces { get; } = new List<IForce2D>();
        public bool NoClipMode { get; } = false;
        public IFixture2D Fixture => _fixture;

        private readonly CircleFixture _fixture;
        
        public CharacterViewModel ViewModel { get; }

        public CharacterData(Color color)
        {
            Color = color;
            _fixture = new CircleFixture(this, Position, Size);
            ViewModel  = new CharacterViewModel
            {
                BoundingBox = GetBoundingBox(),
                HeartBoundingBox = GetHeartBoundingBox(),
                Color = Color,
                HeartColor = HeartColor
            };
        }

        private RectangleF GetBoundingBox()
        {
            return RectangleF.FromCenterAndSize(
                Position,
                Size);
        }

        private RectangleF GetHeartBoundingBox()
        {
            return RectangleF.FromCenterAndSize(
                Position + HeartRelativePosition,
                HeartSize);
        }
    }
}