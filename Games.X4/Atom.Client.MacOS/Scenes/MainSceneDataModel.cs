using System;
using Atom.Client.MacOS.Resources;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Collections;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS
{
    [UsedImplicitly]
    public class MainSceneDataModel
    {
        // 2 cells around player
        public const int AstronomicMapViewRadius = 2;

        public ColorsTexturesPackage ColorsTexturesPackage { get; }
        public MainResourcePackage MainResourcePackage { get; }

        public AstronomicalMap AstronomicalMap { get; private set; }
        
        public bool Initialized { get; private set; }

        public MainSceneDataModel([NotNull] ColorsTexturesPackage colorsTexturesPackage, [NotNull] MainResourcePackage mainResourcePackage)
        {
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));
            MainResourcePackage = mainResourcePackage ?? throw new ArgumentNullException(nameof(mainResourcePackage));
        }

        public void Initialize([NotNull] AstronomicalMap map)
        {
            AstronomicalMap = map ?? throw new ArgumentNullException(nameof(map));
            Initialized = true;
        }
    }
}