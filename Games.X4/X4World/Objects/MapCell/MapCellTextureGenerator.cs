using System;
using System.Collections.Generic;
using System.Threading;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;
using X4World.Maps;

namespace X4World.Objects
{
    public static class MapCellTextureGenerator
    {
        private const int TextureSize = 320;
        
        public static Texture2D GenerateAsync(
            Vector3 cellCenter,
            Vector3 cameraPosition,
            IReadOnlyCollection<WorldMapCellAggregatedData.GalaxyPointData> galaxyPointData,
            ITextureGeneratorService textureGeneratorService,
            IRandomService randomService,
            CancellationToken cancellationToken)
        {
            var texture = textureGeneratorService.EmptyTexture(TextureSize, TextureSize);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var normal = cameraPosition - cellCenter;
                var distance = normal.Length();
                normal.Normalize();

                var count = galaxyPointData.Count;
                var colors = new Color[TextureSize, TextureSize];
                for (int i = 0; i < TextureSize; i++)
                for (int j = 0; j < TextureSize; j++)
                    colors[i, j] = Color.Transparent;
                
                cancellationToken.ThrowIfCancellationRequested();
                
                for (int i = 0; i < count; i++)
                {
                    var x = randomService.NextInteger(0, TextureSize);
                    var y = randomService.NextInteger(0, TextureSize);
                    
                    colors[x, y] = Color.LightYellow;
                }
                
                cancellationToken.ThrowIfCancellationRequested();
                
                texture.SetDataToTexture(colors);
                
                cancellationToken.ThrowIfCancellationRequested();
                // var plane = new Plane(normal, distance);
                // foreach (var pointData in galaxyPointData)
                // {
                //     var globalPosition = pointData.LocalPositionFromCenter + cellCenter;
                //     plane.DotCoordinate()
                // }
            }
            catch (Exception)
            {
                texture?.Dispose();
                throw;
            }
            return texture;
        }
    }
}