using YamlDotNet.Serialization;

namespace X4World.Generation
{
    public class GalaxyGenerationConfig
    {
        [YamlMember(Alias = "mapCell")]
        public MapCellConfig MapCell { get; set; }

        [YamlMember(Alias = "galaxiesBatch")]
        public GalaxiesBatchConfig GalaxiesBatch { get; set; }

        [YamlMember(Alias = "galaxyAsPoint")]
        public GalaxyAsPointConfig GalaxyAsPoint { get; set; }

        [YamlMember(Alias = "galaxyTextureFarthest")]
        public GalaxyTextureFarthestConfig GalaxyTextureFarthest { get; set; }

        [YamlMember(Alias = "galaxyTextureLayered")]
        public GalaxyTextureLayeredConfig GalaxyTextureLayered { get; set; }

        [YamlMember(Alias = "galaxySectorTexture")]
        public GalaxySectorTextureConfig GalaxySectorTexture { get; set; }

        [YamlMember(Alias = "galaxySector")]
        public GalaxySectorConfig GalaxySector { get; set; }

        [YamlMember(Alias = "galaxySectorChunk")]
        public GalaxySectorChunkConfig GalaxySectorChunk { get; set; }

        [YamlMember(Alias = "starSystemsBatch")]
        public StarSystemsBatchConfig StarSystemsBatch { get; set; }

        [YamlMember(Alias = "starSystemAsPoint")]
        public StarSystemAsPointConfig StarSystemAsPoint { get; set; }
    }

    // --- Layer 0: MapCell ---

    public class MapCellConfig
    {
        [YamlMember(Alias = "node")]
        public MapCellNodeConfig Node { get; set; }

        [YamlMember(Alias = "generation")]
        public MapCellGenerationConfig Generation { get; set; }

        [YamlMember(Alias = "debug")]
        public DebugConfig Debug { get; set; }
    }

    public class MapCellNodeConfig
    {
        [YamlMember(Alias = "cellSize")]
        public float CellSize { get; set; }

        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeDepth")]
        public int OctreeDepth { get; set; }
    }

    public class MapCellGenerationConfig
    {
        [YamlMember(Alias = "temperatureMin")]
        public float TemperatureMin { get; set; }

        [YamlMember(Alias = "temperatureMax")]
        public float TemperatureMax { get; set; }

        [YamlMember(Alias = "powerMin")]
        public float PowerMin { get; set; }

        [YamlMember(Alias = "powerMax")]
        public float PowerMax { get; set; }
    }

    public class DebugConfig
    {
        [YamlMember(Alias = "singleGalaxy")]
        public bool SingleGalaxy { get; set; }

        [YamlMember(Alias = "position")]
        public float[] Position { get; set; }

        [YamlMember(Alias = "temperature")]
        public float Temperature { get; set; }

        [YamlMember(Alias = "power")]
        public float Power { get; set; }
    }

    // --- Layer 1: GalaxiesBatch ---

    public class GalaxiesBatchConfig
    {
        [YamlMember(Alias = "node")]
        public GalaxiesBatchNodeConfig Node { get; set; }
    }

    public class GalaxiesBatchNodeConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }

        [YamlMember(Alias = "baseGalaxyHalfSize")]
        public float BaseGalaxyHalfSize { get; set; }
    }

    // --- Layer 2: GalaxyAsPoint ---

    public class GalaxyAsPointConfig
    {
        [YamlMember(Alias = "node")]
        public GalaxyAsPointNodeConfig Node { get; set; }

        [YamlMember(Alias = "generation")]
        public GalaxyAsPointGenerationConfig Generation { get; set; }
    }

    public class GalaxyAsPointNodeConfig
    {
        [YamlMember(Alias = "unwrapDistance")]
        public float UnwrapDistance { get; set; }

        [YamlMember(Alias = "octreeSize")]
        public float OctreeSize { get; set; }

        [YamlMember(Alias = "visualScaleMultiplier")]
        public float VisualScaleMultiplier { get; set; }
    }

    public class GalaxyAsPointGenerationConfig
    {
        [YamlMember(Alias = "baseArmCount")]
        public int BaseArmCount { get; set; }

        [YamlMember(Alias = "armCountPowerScale")]
        public float ArmCountPowerScale { get; set; }

        [YamlMember(Alias = "baseDiskRadius")]
        public float BaseDiskRadius { get; set; }

        [YamlMember(Alias = "diskRadiusPowerScale")]
        public float DiskRadiusPowerScale { get; set; }

        [YamlMember(Alias = "baseClusterCount")]
        public int BaseClusterCount { get; set; }

        [YamlMember(Alias = "clusterCountPowerScale")]
        public float ClusterCountPowerScale { get; set; }

        [YamlMember(Alias = "scatterScaleMin")]
        public float ScatterScaleMin { get; set; }

        [YamlMember(Alias = "scatterScaleMax")]
        public float ScatterScaleMax { get; set; }

        [YamlMember(Alias = "diskThicknessRatio")]
        public float DiskThicknessRatio { get; set; }

        [YamlMember(Alias = "spiralWindFactor")]
        public float SpiralWindFactor { get; set; }

        [YamlMember(Alias = "radialExtentFactor")]
        public float RadialExtentFactor { get; set; }

        [YamlMember(Alias = "baseLuminosityScale")]
        public float BaseLuminosityScale { get; set; }

        [YamlMember(Alias = "luminosityPowerScale")]
        public float LuminosityPowerScale { get; set; }

        [YamlMember(Alias = "luminosityRandomBase")]
        public float LuminosityRandomBase { get; set; }

        [YamlMember(Alias = "temperatureBlend")]
        public float TemperatureBlend { get; set; }
    }

    // --- Layer 3: GalaxyTextureFarthest ---

    public class GalaxyTextureFarthestConfig
    {
        [YamlMember(Alias = "node")]
        public GalaxyTextureFarthestNodeConfig Node { get; set; }

        [YamlMember(Alias = "generation")]
        public GalaxyTextureFarthestGenerationConfig Generation { get; set; }
    }

    public class GalaxyTextureFarthestNodeConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }

        [YamlMember(Alias = "textureBrightnessScale")]
        public float TextureBrightnessScale { get; set; }

        [YamlMember(Alias = "textureStarAlpha")]
        public int TextureStarAlpha { get; set; }

        [YamlMember(Alias = "textureStarRadius")]
        public int TextureStarRadius { get; set; }

        [YamlMember(Alias = "textureBlurRadius")]
        public int TextureBlurRadius { get; set; }

        [YamlMember(Alias = "diffuseDiskBrightness")]
        public float DiffuseDiskBrightness { get; set; }

        [YamlMember(Alias = "diffuseDiskAlpha")]
        public int DiffuseDiskAlpha { get; set; }

        [YamlMember(Alias = "diffuseDiskExtent")]
        public float DiffuseDiskExtent { get; set; }

        [YamlMember(Alias = "bulgeExtent")]
        public float BulgeExtent { get; set; }

        [YamlMember(Alias = "bulgeIntensity")]
        public float BulgeIntensity { get; set; }

        [YamlMember(Alias = "bulgeAlphaScale")]
        public int BulgeAlphaScale { get; set; }
    }

    public class GalaxyTextureFarthestGenerationConfig
    {
        [YamlMember(Alias = "subClustersPerPoint")]
        public int SubClustersPerPoint { get; set; }

        [YamlMember(Alias = "subClusterSpread")]
        public float SubClusterSpread { get; set; }

        [YamlMember(Alias = "subClusterLuminosityScale")]
        public float SubClusterLuminosityScale { get; set; }
    }

    // --- Layer 4: GalaxyTextureLayered ---

    public class GalaxyTextureLayeredConfig
    {
        [YamlMember(Alias = "node")]
        public GalaxyTextureLayeredNodeConfig Node { get; set; }

        [YamlMember(Alias = "generation")]
        public GalaxyTextureLayeredGenerationConfig Generation { get; set; }
    }

    public class GalaxyTextureLayeredNodeConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }

        [YamlMember(Alias = "textureBrightnessScale")]
        public float TextureBrightnessScale { get; set; }

        [YamlMember(Alias = "textureStarAlpha")]
        public int TextureStarAlpha { get; set; }

        [YamlMember(Alias = "textureStarRadius")]
        public int TextureStarRadius { get; set; }

        [YamlMember(Alias = "textureBlurRadius")]
        public int TextureBlurRadius { get; set; }

        [YamlMember(Alias = "diffuseDiskBrightness")]
        public float DiffuseDiskBrightness { get; set; }

        [YamlMember(Alias = "diffuseDiskAlpha")]
        public int DiffuseDiskAlpha { get; set; }

        [YamlMember(Alias = "diffuseDiskExtent")]
        public float DiffuseDiskExtent { get; set; }

        [YamlMember(Alias = "bulgeExtent")]
        public float BulgeExtent { get; set; }

        [YamlMember(Alias = "bulgeIntensity")]
        public float BulgeIntensity { get; set; }

        [YamlMember(Alias = "bulgeAlphaScale")]
        public int BulgeAlphaScale { get; set; }
    }

    public class GalaxyTextureLayeredGenerationConfig
    {
        [YamlMember(Alias = "sectorGridSize")]
        public int SectorGridSize { get; set; }

        [YamlMember(Alias = "subClustersPerPoint")]
        public int SubClustersPerPoint { get; set; }

        [YamlMember(Alias = "subClusterSpread")]
        public float SubClusterSpread { get; set; }

        [YamlMember(Alias = "subClusterLuminosityScale")]
        public float SubClusterLuminosityScale { get; set; }
    }

    // --- Layer 5: GalaxySectorTexture ---

    public class GalaxySectorTextureConfig
    {
        [YamlMember(Alias = "node")]
        public GalaxySectorTextureNodeConfig Node { get; set; }

        [YamlMember(Alias = "generation")]
        public GalaxySectorTextureGenerationConfig Generation { get; set; }
    }

    public class GalaxySectorTextureNodeConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }
    }

    public class GalaxySectorTextureGenerationConfig
    {
        [YamlMember(Alias = "diskThicknessScale")]
        public float DiskThicknessScale { get; set; }
    }

    // --- Layer 6: GalaxySector ---

    public class GalaxySectorConfig
    {
        [YamlMember(Alias = "node")]
        public GalaxySectorNodeConfig Node { get; set; }

        [YamlMember(Alias = "generation")]
        public GalaxySectorGenerationConfig Generation { get; set; }
    }

    public class GalaxySectorNodeConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }

        [YamlMember(Alias = "dotBaseRadius")]
        public float DotBaseRadius { get; set; }

        [YamlMember(Alias = "dotRadiusScale")]
        public float DotRadiusScale { get; set; }

        [YamlMember(Alias = "dotBaseBrightness")]
        public float DotBaseBrightness { get; set; }

        [YamlMember(Alias = "dotBrightnessScale")]
        public float DotBrightnessScale { get; set; }

        [YamlMember(Alias = "dotEdgeBrightness")]
        public float DotEdgeBrightness { get; set; }
    }

    public class GalaxySectorGenerationConfig
    {
        [YamlMember(Alias = "maxPointsPerChunk")]
        public int MaxPointsPerChunk { get; set; }

        [YamlMember(Alias = "maxSplitDepth")]
        public int MaxSplitDepth { get; set; }
    }

    // --- Layer 7: GalaxySectorChunk ---

    public class GalaxySectorChunkConfig
    {
        [YamlMember(Alias = "node")]
        public GalaxySectorChunkNodeConfig Node { get; set; }

        [YamlMember(Alias = "generation")]
        public GalaxySectorChunkGenerationConfig Generation { get; set; }
    }

    public class GalaxySectorChunkNodeConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "minUnwrapDistance")]
        public float MinUnwrapDistance { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }

        [YamlMember(Alias = "dotBaseRadius")]
        public float DotBaseRadius { get; set; }

        [YamlMember(Alias = "dotRadiusScale")]
        public float DotRadiusScale { get; set; }

        [YamlMember(Alias = "dotBaseBrightness")]
        public float DotBaseBrightness { get; set; }

        [YamlMember(Alias = "dotBrightnessScale")]
        public float DotBrightnessScale { get; set; }

        [YamlMember(Alias = "dotEdgeBrightness")]
        public float DotEdgeBrightness { get; set; }

        [YamlMember(Alias = "densityDampingReference")]
        public float DensityDampingReference { get; set; }
    }

    public class GalaxySectorChunkGenerationConfig
    {
        [YamlMember(Alias = "maxPointsPerBatch")]
        public int MaxPointsPerBatch { get; set; }

        [YamlMember(Alias = "maxSplitDepth")]
        public int MaxSplitDepth { get; set; }
    }

    // --- Layer 8: StarSystemsBatch ---

    public class StarSystemsBatchConfig
    {
        [YamlMember(Alias = "node")]
        public StarSystemsBatchNodeConfig Node { get; set; }
    }

    public class StarSystemsBatchNodeConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "minUnwrapDistance")]
        public float MinUnwrapDistance { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }

        [YamlMember(Alias = "dotBaseRadius")]
        public float DotBaseRadius { get; set; }

        [YamlMember(Alias = "dotRadiusScale")]
        public float DotRadiusScale { get; set; }

        [YamlMember(Alias = "dotBaseBrightness")]
        public float DotBaseBrightness { get; set; }

        [YamlMember(Alias = "dotBrightnessScale")]
        public float DotBrightnessScale { get; set; }

        [YamlMember(Alias = "dotBrightnessMin")]
        public float DotBrightnessMin { get; set; }

        [YamlMember(Alias = "dotBrightnessMax")]
        public float DotBrightnessMax { get; set; }

        [YamlMember(Alias = "dotGlowAlphaScale")]
        public float DotGlowAlphaScale { get; set; }

        [YamlMember(Alias = "dotGlowRadiusScale")]
        public float DotGlowRadiusScale { get; set; }
    }

    // --- Layer 9: StarSystemAsPoint ---

    public class StarSystemAsPointConfig
    {
        [YamlMember(Alias = "node")]
        public StarSystemAsPointNodeConfig Node { get; set; }
    }

    public class StarSystemAsPointNodeConfig
    {
        [YamlMember(Alias = "visualScaleMultiplier")]
        public float VisualScaleMultiplier { get; set; }

        [YamlMember(Alias = "brightnessMultiplier")]
        public float BrightnessMultiplier { get; set; }

        [YamlMember(Alias = "brightnessMin")]
        public float BrightnessMin { get; set; }

        [YamlMember(Alias = "brightnessMax")]
        public float BrightnessMax { get; set; }
    }
}
