using System;
using FrameworkSDK.MonoGame.Graphics.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.Graphics
{
    public class GalaxySlabMaterial : IMeshMaterial
    {
        private readonly Func<Vector3> _getCameraPosition;
        private readonly float _brightness;
        private readonly float _falloffSharpness;
        private readonly Vector2 _sectorUVOffset;
        private readonly float _sectorUVScale;
        private readonly float _edgeFadeStart;

        public Texture2D GalaxyTexture { get; set; }

        public GalaxySlabMaterial(
            Func<Vector3> getCameraPosition,
            float brightness,
            float falloffSharpness,
            Vector2 sectorUVOffset,
            float sectorUVScale,
            float edgeFadeStart)
        {
            _getCameraPosition = getCameraPosition;
            _brightness = brightness;
            _falloffSharpness = falloffSharpness;
            _sectorUVOffset = sectorUVOffset;
            _sectorUVScale = sectorUVScale;
            _edgeFadeStart = edgeFadeStart;
        }

        public void ApplyToEffect(Effect effect)
        {
            var slabEffect = (GalaxySlabEffect)effect;
            slabEffect.SetCameraPosition(_getCameraPosition());
            slabEffect.SetBrightness(_brightness);
            slabEffect.SetFalloffSharpness(_falloffSharpness);
            slabEffect.SetSectorUVOffset(_sectorUVOffset);
            slabEffect.SetSectorUVScale(_sectorUVScale);
            slabEffect.SetEdgeFadeStart(_edgeFadeStart);

            if (GalaxyTexture != null)
                slabEffect.SetGalaxyTexture(GalaxyTexture);
        }
    }
}
