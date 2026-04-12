using System;
using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Generation;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxySectorSlabViewModel3D : ViewModel3D
    {
        public GalaxySectorTextureAggregatedData AggregatedData { get; }
        public GalaxyTextureLayeredAggregatedData ParentAggregatedData { get; }

        public Vector2 SectorUVOffset { get; }
        public float SectorUVScale { get; }

        public event Action<Texture2D> SlabTextureReady;

        public GalaxySectorSlabViewModel3D(
            GalaxySectorTextureAggregatedData sectorAgg,
            GalaxyTextureLayeredAggregatedData galaxyAgg,
            Vector3 worldPosition)
        {
            AggregatedData = sectorAgg;
            ParentAggregatedData = galaxyAgg;

            Position = worldPosition;

            var sectorDiameter = sectorAgg.SectorRadius * 2f;
            var genCfg = GalaxyConfig.Instance.GalaxyAsPoint.Generation;
            var cfg = GalaxyConfig.Instance.GalaxySectorTexture.Node;
            var ySize = sectorDiameter * genCfg.DiskThicknessRatio * cfg.SlabThicknessScale;
            Scale = new Vector3(sectorDiameter, ySize, sectorDiameter);

            Rotation = Matrix.CreateRotationX(sectorAgg.Inclination)
                     * Matrix.CreateRotationY(sectorAgg.SpinAngle);
            GraphicsPassName = GraphicsPasses.GalaxySlab;

            var diskRadius = sectorAgg.DiskRadius;
            SectorUVScale = sectorAgg.SectorRadius / diskRadius;
            SectorUVOffset = new Vector2(
                sectorAgg.SectorCenterX / (2f * diskRadius) + 0.5f,
                sectorAgg.SectorCenterZ / (2f * diskRadius) + 0.5f);
        }

        public void OnSlabTextureAvailable(Texture2D texture)
        {
            SlabTextureReady?.Invoke(texture);
        }
    }
}
