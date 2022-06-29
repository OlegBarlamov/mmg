using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Materials
{
    public class TextureMaterial : IMeshMaterial
    {
        public Texture2D Texture { get; }
        private string EffectTextureParameterName { get; }

        public TextureMaterial([NotNull] Texture2D texture, [NotNull] string effectTextureParameterName = "Texture")
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            EffectTextureParameterName = effectTextureParameterName ?? throw new ArgumentNullException(nameof(effectTextureParameterName));
        }

        public void ApplyToEffect(Effect effect)
        {
            ((BasicEffect) effect).Texture = Texture;
            //effect.Parameters[EffectTextureParameterName].SetValue(Texture);
        }
    }
}