using System;
using System.Linq;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;
using MonoGameExtensions;

namespace FrameworkSDK.Game.Scenes
{
    internal static class ViewMappingCollectionExtension
    {
        public static bool ContainsView([NotNull] this UpdatableCollection<ViewMapping> collection, [NotNull] IView view)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (view == null) throw new ArgumentNullException(nameof(view));

            var items = collection.GetAllWithToAddItems();
            return items.Any(mapping => mapping.View == view);
        }
    }
}
