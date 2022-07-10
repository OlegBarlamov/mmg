using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources
{
    internal class DefaultResourcesService : IDefaultResourcesService, IDisposable
    {
        private ITextureGeneratorApi TextureGeneratorApi { get; }

        private IDictionary<Type, object> DefaultResourceMap => _lazyDefaultResourcesMap.Value;
        
        private readonly Lazy<IDictionary<Type, object>> _lazyDefaultResourcesMap;
        
        public DefaultResourcesService([NotNull] ITextureGeneratorApi textureGeneratorApi)
        {
            TextureGeneratorApi = textureGeneratorApi ?? throw new ArgumentNullException(nameof(textureGeneratorApi));

            _lazyDefaultResourcesMap = new Lazy<IDictionary<Type, object>>(GenerateDefaultResourcesMap, LazyThreadSafetyMode.PublicationOnly);
        }

        public bool HasDefaultVersionFor<T>()
        {
            return DefaultResourceMap.ContainsKey(typeof(T));
        }
        
        public T GetDefaultVersionFor<T>()
        {
            return (T)DefaultResourceMap[typeof(T)];
        }
        
        private IDictionary<Type, object> GenerateDefaultResourcesMap()
        {
            var defaultTexture = TextureGeneratorApi.DiffuseColor(Color.Magenta);
            var defaultSpriteFont = GenerateSpriteFont(defaultTexture);
            
            return new Dictionary<Type, object>
            {
                { typeof(Texture2D), defaultTexture },
                { typeof(SpriteFont), defaultSpriteFont }
            };
        }

        private static SpriteFont GenerateSpriteFont(Texture2D texture)
        {
            var characters = Enumerable.Range(0, 255).Select(i => (char) i).ToList();
            var glyphs = Enumerable.Repeat(new Rectangle(0, 0, 1, 1), characters.Count).ToList();
            var cropping = Enumerable.Repeat(new Rectangle(0, 0, 1, 1), characters.Count).ToList();
            var kerning = Enumerable.Repeat(Vector3.Zero, characters.Count).ToList();
            return new SpriteFont(texture, glyphs, cropping, characters, 1, 1, kerning, null);
        }

        public void Dispose()
        {
            if (_lazyDefaultResourcesMap.IsValueCreated)
            {
                foreach (var resource in _lazyDefaultResourcesMap.Value.Values)
                {
                    using (resource as IDisposable)
                    {
                    }
                }
            }
        }
    }
}