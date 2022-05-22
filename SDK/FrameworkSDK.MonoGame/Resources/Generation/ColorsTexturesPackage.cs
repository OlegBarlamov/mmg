using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    public class ColorsTexturesPackage : ResourcePackage
    {
        public Texture2D Get(Color color)
        {
            return _textures[color];
        }
        
        private readonly Dictionary<Color, Texture2D> _textures = new Dictionary<Color, Texture2D>();

        protected override void Load(IContentLoaderApi content)
        {
            var colors = typeof(Color)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Select(x => (Color)x.GetValue(null))
                .ToArray();
            
            foreach (var color in colors)
            {
                var texture = content.DiffuseColor(color);
                _textures[color] = texture;
            }
        }
    }
}