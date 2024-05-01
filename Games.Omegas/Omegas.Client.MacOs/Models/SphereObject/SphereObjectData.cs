using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics._2D.BodyTypes;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using Omegas.Client.MacOs.Services;
using SimplePhysics2D.Fixtures;

namespace Omegas.Client.MacOs.Models.SphereObject
{
    public class SphereObjectData : IColliderBody2D 
    {
        public Teams Team { get; }
        
        public Color Color { get; }
        
        public bool Dead { get; set; }

        public Vector2 Position { get; private set; }

        public virtual void SetPosition(Vector2 position)
        {
            if (Double.IsNaN(position.X) || Double.IsInfinity(position.X))
            {
                return;
            }

            Position = position;
            ViewModel.BoundingBox = GetBoundingBox();
            _fixture.Center = Position;
        }

        public float Rotation { get; private set; }
        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }
        
        public float Size { get; private set; }

        public void SetSize(float size)
        {
            Size = size;
            ViewModel.BoundingBox = GetBoundingBox();
            _fixture.Radius = size;
            _parameters.Mass = Size / 10f;
        }

        public IPhysicsBody2DParameters Parameters => _parameters;
        private readonly DynamicBody2DParameters _parameters  = new DynamicBody2DParameters
        {
            FrictionFactor = 0.02f
        };
        
        public IScene2DPhysics Scene { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public ICollection<IForce2D> ActiveForces { get; } = new List<IForce2D>();
        public bool NoClipMode { get; set; }
        public IFixture2D Fixture => _fixture;
        
        public List<SphereObjectData> CollidingSpheres { get; } = new List<SphereObjectData>();
        
        public bool OnCollision(IColliderBody2D body)
        {
            if (body is SphereObjectData sphereObjectData)
            {
                if (!CollidingSpheres.Contains(sphereObjectData))
                    CollidingSpheres.Add(sphereObjectData);
            }

            return false;
        }

        private readonly CircleFixture _fixture;
        
        public virtual SphereObjectViewModel ViewModel { get; }

        public SphereObjectData(Color color, Vector2 position, float size, Teams team)
        {
            Size = size;
            Position = position;
            Color = color;
            Team = team;
            
            _fixture = new CircleFixture(this, Position, Size);
            _parameters.Mass = Size / 10f;

            ViewModel  = new SphereObjectViewModel
            {
                BoundingBox = GetBoundingBox(),
                Color = Color,
            };
        }

        protected RectangleF GetBoundingBox()
        {
            return RectangleF.FromCenterAndSize(
                Position,
                Size);
        }
    }
}