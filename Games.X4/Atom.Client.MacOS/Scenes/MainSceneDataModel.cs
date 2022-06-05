using System;
using Atom.Client.MacOS.Resources;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Collections;
using NetExtensions.Geometry;
using X4World;
using X4World.Maps;

namespace Atom.Client.MacOS
{
    [UsedImplicitly]
    public class MainSceneDataModel
    {
        // 2 cells around player
        public const int AstronomicMapViewRadius = 2;

        public ColorsTexturesPackage ColorsTexturesPackage { get; }
        public MainResourcePackage MainResourcePackage { get; }

        public GalaxiesMap GalaxiesMap { get; private set; }
        
        public bool Initialized { get; private set; }

        public MainSceneDataModel([NotNull] ColorsTexturesPackage colorsTexturesPackage, [NotNull] MainResourcePackage mainResourcePackage)
        {
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));
            MainResourcePackage = mainResourcePackage ?? throw new ArgumentNullException(nameof(mainResourcePackage));
        }

        public void Initialize([NotNull] GalaxiesMap map)
        {
            GalaxiesMap = map ?? throw new ArgumentNullException(nameof(map));
            Initialized = true;
        }
    }
}