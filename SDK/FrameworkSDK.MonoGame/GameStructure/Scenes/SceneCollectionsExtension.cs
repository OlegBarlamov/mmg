using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.GameStructure.Scenes
{
    internal static class SceneCollectionsExtension
    {
        public static bool ContainsView([NotNull] this UpdatableCollection<ViewMapping> collection, [NotNull] IView view)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (view == null) throw new ArgumentNullException(nameof(view));

            var items = collection.GetAllWithToAddItems();
            return items.Any(mapping => mapping.View == view);
        }

        public static IReadOnlyCollection<IGraphicComponent> Components([NotNull] this UpdatableCollection<ViewMapping> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            return collection.Select(mapping => mapping.View).ToArray();
        }

        public static void Update([NotNull] this UpdatableCollection<IController> collection, [NotNull] GameTime gameTime)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (gameTime == null) throw new ArgumentNullException(nameof(gameTime));

            foreach (var o in collection)
            {
                o.Update(gameTime);
            }
        }
    }
}
