using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics2D;

namespace SimplePhysics2D
{
    public interface ICollidersSpace2D
    {
        void AddBody(IColliderBody2D body);
        void RemoveBody(IColliderBody2D body);

        IEnumerable<IColliderBody2D> GetCollisionCandidates(IColliderBody2D body);
    }
}