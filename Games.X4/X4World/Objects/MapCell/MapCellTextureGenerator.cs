using System;
using System.Collections.Generic;
using System.Threading;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace X4World.Objects
{
    public static class MapCellTextureGenerator
    {
        private const int TextureSize = 32;
        
        public static Texture2D GenerateAsync(
            Vector3 cellCenter,
            Vector3 cameraPosition,
            IReadOnlyCollection<WorldMapCellAggregatedData.GalaxyPointData> galaxyPointData,
            ITextureGeneratorService textureGeneratorService,
            CancellationToken cancellationToken)
        {
            var texture = textureGeneratorService.EmptyTexture(TextureSize, TextureSize);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var colors = new Color[TextureSize, TextureSize];
                for (int i = 0; i < TextureSize; i++)
                for (int j = 0; j < TextureSize; j++)
                    colors[i, j] = Color.Transparent;
                
                cancellationToken.ThrowIfCancellationRequested();
                
                var normal = Vector3.Normalize(cameraPosition - cellCenter);

                foreach (var pointData in galaxyPointData)
                {
                    var globalPointPosition = pointData.LocalPositionFromCenter + cellCenter;
                    
                    var distanceVector = globalPointPosition - cellCenter;
                    var directionFromPointToCamera = Vector3.Normalize(cameraPosition - globalPointPosition);
                    var dist = Vector3.Dot(distanceVector, directionFromPointToCamera);
                    var projectedPoint = globalPointPosition - dist * directionFromPointToCamera;

                    var v = Vector3.Normalize(Vector3.Cross(normal, Vector3.Up));
                    if (v == Vector3.Zero)
                        v = Vector3.Normalize(Vector3.Cross(normal, Vector3.UnitX));
                    var u = -Vector3.Normalize(Vector3.Cross(normal, v));

                    var uTextureCoordinateLength = Vector3.Dot(u, projectedPoint - cellCenter);
                    var vTextureCoordinateLength = Vector3.Dot(v, projectedPoint - cellCenter);

                    var uTextureCoordinate = (uTextureCoordinateLength + WorldConstants.WorldMapCellSize / 2) /
                        WorldConstants.WorldMapCellSize * TextureSize;
                    var vTextureCoordinate = (vTextureCoordinateLength + WorldConstants.WorldMapCellSize / 2) /
                        WorldConstants.WorldMapCellSize * TextureSize;
                    
                    if (uTextureCoordinate >= 0 && uTextureCoordinate < TextureSize &&
                        vTextureCoordinate >= 0 && vTextureCoordinate < TextureSize)
                    {
                        colors[(int) uTextureCoordinate, (int) vTextureCoordinate] = Color.Yellow;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
                
                texture.SetDataToTexture(colors);
                
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                texture?.Dispose();
                throw;
            }
            return texture;
        }
    }
}