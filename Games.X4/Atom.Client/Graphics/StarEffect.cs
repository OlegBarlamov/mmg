using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.Graphics
{
    public class StarEffect : Effect, IEffectMatrices
    {
        private readonly EffectParameter _worldParam;
        private readonly EffectParameter _viewParam;
        private readonly EffectParameter _projectionParam;
        private readonly EffectParameter _starColorParam;
        private readonly EffectParameter _intensityParam;

        private Matrix _world = Matrix.Identity;
        private Matrix _view = Matrix.Identity;
        private Matrix _projection = Matrix.Identity;

        public StarEffect(Effect cloneSource) : base(cloneSource)
        {
            _worldParam = Parameters["World"];
            _viewParam = Parameters["View"];
            _projectionParam = Parameters["Projection"];
            _starColorParam = Parameters["StarColor"];
            _intensityParam = Parameters["Intensity"];
        }

        public Matrix World
        {
            get => _world;
            set
            {
                _world = value;
                _worldParam.SetValue(value);
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

        public void SetStarColor(Color color)
        {
            _starColorParam.SetValue(color.ToVector4());
        }

        public void SetIntensity(float intensity)
        {
            _intensityParam.SetValue(intensity);
        }
    }
}
