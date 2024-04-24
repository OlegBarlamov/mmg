using System;
using FrameworkSDK.MonoGame;

namespace SimplePhysics2D
{
    public class Physics2DException : FrameworkMonoGameException
    {
        internal Physics2DException(string message) : base(message)
        {
        }

        internal Physics2DException(string message, Exception inner) : base(message, inner)
        {
        }

        internal Physics2DException(string message, Exception inner, params object[] args) : base(message, inner, args)
        {
        }

        internal Physics2DException(string message, params object[] args) : base(message, args)
        {
        }
    }
}