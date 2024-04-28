using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics._2D.BodyTypes;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using SimplePhysics2D.Fixtures;

namespace SimplePhysics2D.Objects
{
    public class PhysicsMapBounds2D : IColliderBody2D
    {
        public RectangleF Rectangle => _rectangle;
        private RectangleF _rectangle;

        public IFixture2D Fixture => _fixture;
        private readonly Map2DBoundsFixture _fixture;
        
        public Vector2 Position => Rectangle.Location;
        public void SetPosition(Vector2 position)
        {
            _rectangle.Location = position;
            _fixture.Rectangle = _rectangle;
        }

        public float Rotation { get; } = 0f;
        public void SetRotation(float rotation)
        {
        }
        
        public IScene2DPhysics Scene { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }

        public IPhysicsBody2DParameters Parameters { get; }
        public ICollection<IForce2D> ActiveForces { get; } = new List<IForce2D>();
        
        public bool NoClipMode { get; } = false;

        public PhysicsMapBounds2D(RectangleF rectangle):this(rectangle, new StaticBody2DParameters())
        {
        }

        public PhysicsMapBounds2D(RectangleF rectangle, [NotNull] IPhysicsBody2DParameters parameters)
        {
            _rectangle = rectangle;
            _fixture = new Map2DBoundsFixture(this, rectangle);
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }
    }
}