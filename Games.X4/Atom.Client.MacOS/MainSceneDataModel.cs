using System;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using MonoGameExtensions.Geometry;
using NetExtensions.Collections;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS
{
    [UsedImplicitly]
    public class MainSceneDataModel
    {
        public ColorsTexturesPackage ColorsTexturesPackage { get; }

        public AstronomicalMap AstronomicalMap { get; }

        public MainSceneDataModel([NotNull] ColorsTexturesPackage colorsTexturesPackage, IRandomService randomService)
        {
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));

            AstronomicalMap = new AstronomicalMap();

            var generator = new AstronomicalMapGenerator(randomService);
            EnumerableExtended.For(-1, 1, -1, 1, -1, 1, (i, j, k) =>
            {
                var point = new Point3D(i, j, k);
                var cell = generator.GenerateCell(point);
                AstronomicalMap.SetCell(point, cell);
            });
        }
    }
}