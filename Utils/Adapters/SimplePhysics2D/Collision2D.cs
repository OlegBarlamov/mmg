using System;
using FrameworkSDK.MonoGame.Physics2D;
using Microsoft.Xna.Framework;

namespace SimplePhysics2D
{
    public struct Collision2D
    {
        public static Collision2D Empty { get; } = new Collision2D(true);

        public readonly bool IsEmpty;
        
        public Vector2 Point;

        public Vector2 PenetrationVector;

        public IFixture2D FixtureA;
        
        public IFixture2D FixtureB;

        private Collision2D(bool isEmpty)
        {
            IsEmpty = isEmpty;
            FixtureA = null;
            FixtureB = null;
            PenetrationVector = Vector2.Zero;
            Point = Vector2.Zero;
        }

        internal Collision2D(IFixture2D fixtureA, IFixture2D fixtureB, Vector2 point, Vector2 penetrationVector)
        {
            IsEmpty = false;
            FixtureA = fixtureA;
            FixtureB = fixtureB;
            Point = point;
            PenetrationVector = penetrationVector;
        }
    }
}