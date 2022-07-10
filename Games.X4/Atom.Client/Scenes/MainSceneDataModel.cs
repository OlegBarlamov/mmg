using Atom.Client.Resources;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using System;
using X4World.Maps;

namespace Atom.Client.Scenes
{
    [UsedImplicitly]
    public class MainSceneDataModel
    {
        public ColorsTexturesPackage ColorsTexturesPackage { get; }
        public MainResourcePackage MainResourcePackage { get; }

        public GlobalWorldMap GlobalWorldMap { get; private set; }
        
        public bool Initialized { get; private set; }

        public MainSceneDataModel([NotNull] ColorsTexturesPackage colorsTexturesPackage, [NotNull] MainResourcePackage mainResourcePackage)
        {
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));
            MainResourcePackage = mainResourcePackage ?? throw new ArgumentNullException(nameof(mainResourcePackage));
        }

        public void Initialize([NotNull] GlobalWorldMap grid)
        {
            GlobalWorldMap = grid ?? throw new ArgumentNullException(nameof(grid));
            Initialized = true;
        }
    }
}