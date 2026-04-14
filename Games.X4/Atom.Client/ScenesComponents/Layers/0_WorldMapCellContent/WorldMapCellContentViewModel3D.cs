using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class WorldMapCellContentViewModel3D : ViewModel3D
    {
        public WorldMapCellAggregatedData WorldMapCellAggregatedData { get; }
        public Vector3 WorldPosition { get; }

        public WorldMapCellContentViewModel3D(WorldMapCellContent model)
        {
            WorldMapCellAggregatedData = model.WorldMapCellAggregatedData;
            WorldPosition = model.GetWorldPosition();
            Position = WorldPosition;
            Scale = new Vector3(model.Size);
            GraphicsPassName = GraphicsPasses.TexturedNoLights;
        }
    }
}
