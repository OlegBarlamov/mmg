namespace X4World
{
    /// <summary>
    /// Approaches about the world storing and processing.
    ///
    /// What is important:
    /// 1) How to find objects to render.
    /// 2) How to handle objects movements/changes.
    /// 3) How to auto-generate world.
    /// 
    ///
    /// Approaches:
    /// 1) High level grid -> space fixed quad-tree with objects inside
    /// Problem: Impossible to handle huge objects movements (galaxies). It needs to rebuild the huge branches of the tree when galaxy
    /// goes from one node of the fixed quad tree to another.
    /// Inference: Quad tree nodes should be assigned to objects.
    ///
    /// 2) High level grid -> Objects related quad-trees. Example:
    /// Cell [0, 0] -> ~10-20 of GalaxyGroup: Galaxies tuning around a fixed point {Galaxies as points or spots} ->
    ///                ~1-20  of Galaxy: Galaxies, the actor itself is turning around the center {Galaxies models as sprites and models} ->
    ///                ~
    ///
    /// 3) High level grid -> Objects related quad-trees based on objects number. (Collisions quad tree) Example:
    /// Cell [0, 0] -> GalaxyGroup Octree. Less then 10 objects per node: Galaxies tuning around a fixed point {Galaxies as points or spots + nebulas} ->
    ///                Galaxies Ocree: Galaxies, the actor itself is turning around the center of the group {Galaxies models as sprites and models} ->
    ///                Stars Octree
    ///                StarSystem octree
    ///
    /// 1+; 2+; 3+
    /// How to find objects to render?
    ///  Players node: [Several planets, Moon]
    ///    Parent (width check): [nothing]
    ///  Children: [Other planets]
    ///    Parent [nothing]
    ///      Parent [nothing]
    ///  
    /// </summary>
    public static class WorldConstants
    {
        public const float GalaxiesMapCellSize = 100f;
    }
}