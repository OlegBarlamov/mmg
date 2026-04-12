using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.Graphics
{
    public class GalaxySlabEffect : Effect, IEffectMatrices
    {
        private readonly EffectParameter _worldParam;
        private readonly EffectParameter _viewParam;
        private readonly EffectParameter _projectionParam;
        private readonly EffectParameter _worldInverseParam;
        private readonly EffectParameter _cameraPositionParam;
        private readonly EffectParameter _brightnessParam;
        private readonly EffectParameter _falloffSharpnessParam;
        private readonly EffectParameter _sectorUVOffsetParam;
        private readonly EffectParameter _sectorUVScaleParam;
        private readonly EffectParameter _edgeFadeStartParam;
        private readonly EffectParameter _galaxyTextureParam;

        private Matrix _world = Matrix.Identity;
        private Matrix _view = Matrix.Identity;
        private Matrix _projection = Matrix.Identity;

        public GalaxySlabEffect(Effect cloneSource) : base(cloneSource)
        {
            _worldParam = Parameters["World"];
            _viewParam = Parameters["View"];
            _projectionParam = Parameters["Projection"];
            _worldInverseParam = Parameters["WorldInverse"];
            _cameraPositionParam = Parameters["CameraPositionWS"];
            _brightnessParam = Parameters["Brightness"];
            _falloffSharpnessParam = Parameters["FalloffSharpness"];
            _sectorUVOffsetParam = Parameters["SectorUVOffset"];
            _sectorUVScaleParam = Parameters["SectorUVScale"];
            _edgeFadeStartParam = Parameters["EdgeFadeStart"];
            _galaxyTextureParam = Parameters["GalaxyTexture"];
        }

        public Matrix World
        {
            get => _world;
            set
            {
                _world = value;
                _worldParam.SetValue(value);
                _worldInverseParam.SetValue(Matrix.Invert(value));
            }
        }

        public Matrix View
        {
            get => _view;
            set
            {
                _view = value;
                _viewParam.SetValue(value);
            }
        }

        public Matrix Projection
        {
            get => _projection;
            set
            {
                _projection = value;
                _projectionParam.SetValue(value);
            }
        }

        public void SetCameraPosition(Vector3 position)
        {
            _cameraPositionParam.SetValue(position);
        }

        public void SetBrightness(float brightness)
        {
            _brightnessParam.SetValue(brightness);
        }

        public void SetFalloffSharpness(float sharpness)
        {
            _falloffSharpnessParam.SetValue(sharpness);
        }

        public void SetSectorUVOffset(Vector2 offset)
        {
            _sectorUVOffsetParam.SetValue(offset);
        }

        public void SetSectorUVScale(float scale)
        {
            _sectorUVScaleParam.SetValue(scale);
        }

        public void SetEdgeFadeStart(float start)
        {
            _edgeFadeStartParam.SetValue(start);
        }

        public void SetGalaxyTexture(Texture2D texture)
        {
            _galaxyTextureParam.SetValue(texture);
        }
    }
}
