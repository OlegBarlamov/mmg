using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Objects;

namespace X4World.Maps
{
    public class GlobalWorldMapCellContent
    {
        public bool IsGenerated { get; }
        
        // All data which are needed to generate content
        public float SubstanceAmount { get; }
        
        public AutoSplitOctreeNode<IGalaxiesLevelObject> GalaxiesLevelObjects { get; internal set; }

        public GlobalWorldMapCellContent(Vector3 center, float size)
        {
            GalaxiesLevelObjects = new AutoSplitOctreeNode<IGalaxiesLevelObject>(center, size, 10);
        }
    }
}