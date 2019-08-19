using System;
using System.Linq;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace FrameworkSDK.Game.Scenes
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

        public static void Draw([NotNull] this UpdatableCollection<ViewMapping> collection, [NotNull] GameTime gameTime)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (gameTime == null) throw new ArgumentNullException(nameof(gameTime));

            var views = collection.Select(mapping => mapping.View);
            foreach (var view in views)
            {
                view.Draw(gameTime);
            }
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
