using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RenderPassAttribute : Attribute
    {
        public string PassName { get; }

        public RenderPassAttribute([NotNull] string passName)
        {
            PassName = passName ?? throw new ArgumentNullException(nameof(passName));
        }
    }
}