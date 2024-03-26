using System;

namespace FrameworkSDK.MonoGame.Mvc
{
    public class SceneNotInitializedException : ObjectNotInitializedException
    {
        internal SceneNotInitializedException(string message) : base(message)
        {
        }

        internal SceneNotInitializedException(string message, Exception inner) : base(message, inner)
        {
        }

        internal SceneNotInitializedException(string message, Exception inner, params object[] args) : base(message, inner, args)
        {
        }

        internal SceneNotInitializedException(string message, params object[] args) : base(message, args)
        {
        }
    }
}