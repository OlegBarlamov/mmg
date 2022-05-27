using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.MacOS.Components
{
    public class Grid3DComponentData 
    {
        
    }

    public class Grid3DComponentView : SingleMeshComponent<Nothing>
    {
        protected override IRenderableMesh Mesh { get; } 
        protected override string SingleGraphicsPassName { get; } = "render";

        public Grid3DComponentView()
        {
            Mesh = new FixedSimpleMesh<VertexPositionColor>(this, PrimitiveType.LineList,
                VertexPositionColor.VertexDeclaration, new[]
                {
                    new VertexPositionColor(new Vector3(10, 0, 0), Color.Red),
                    new VertexPositionColor(new Vector3(0, 0, 0), Color.Red),

                    new VertexPositionColor(new Vector3(0, 0, 10), Color.Green),
                    new VertexPositionColor(new Vector3(0, 0, 0), Color.Green),

                    new VertexPositionColor(new Vector3(0, 10, 0), Color.Blue),
                    new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue),
                }, new[] {0, 1, 2, 3, 4, 5}, 6);
        }
    }
}