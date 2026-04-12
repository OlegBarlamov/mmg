using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarSystemAsLightPointViewModel3D : ViewModel3D
    {
        public StarSystemAsLightPointViewModel3D(StarSystemAsLightPoint starSystem)
        {
            var cfg = GalaxyConfig.Instance.StarSystemAsLightPoint.Node;
            Position = starSystem.GetWorldPosition();
            Scale = new Vector3(starSystem.AggregatedData.Luminosity * cfg.VisualScaleMultiplier);
            MeshMaterial = new StarMaterial(starSystem.AggregatedData.Color,
                MathHelper.Clamp(starSystem.AggregatedData.Luminosity * cfg.BrightnessMultiplier, cfg.BrightnessMin, cfg.BrightnessMax));
            GraphicsPassName = GraphicsPasses.Stars;
        }
    }
}
