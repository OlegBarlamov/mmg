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

    public class MapCellConfig
    {
        [YamlMember(Alias = "cellSize")]
        public float CellSize { get; set; }

        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeDepth")]
        public int OctreeDepth { get; set; }

        [YamlMember(Alias = "galaxyGeneration")]
        public GalaxyGenerationParams GalaxyGeneration { get; set; }

        [YamlMember(Alias = "debug")]
        public DebugConfig Debug { get; set; }
    }

    public class GalaxyGenerationParams
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

    public class GalaxiesBatchConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }

        [YamlMember(Alias = "baseGalaxyHalfSize")]
        public float BaseGalaxyHalfSize { get; set; }
    }

    public class GalaxyAsPointConfig
    {
        [YamlMember(Alias = "unwrapDistance")]
        public float UnwrapDistance { get; set; }

        [YamlMember(Alias = "octreeSize")]
        public float OctreeSize { get; set; }

        [YamlMember(Alias = "visualScaleMultiplier")]
        public float VisualScaleMultiplier { get; set; }

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

    public class GalaxyTextureFarthestConfig
    {
        [YamlMember(Alias = "subClustersPerPoint")]
        public int SubClustersPerPoint { get; set; }

        [YamlMember(Alias = "subClusterSpread")]
        public float SubClusterSpread { get; set; }

        [YamlMember(Alias = "subClusterLuminosityScale")]
        public float SubClusterLuminosityScale { get; set; }

        [YamlMember(Alias = "sectorGridSize")]
        public int SectorGridSize { get; set; }

        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }
    }

    public class GalaxyTextureLayeredConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }
    }

    public class GalaxySectorTextureConfig
    {
        [YamlMember(Alias = "diskThicknessScale")]
        public float DiskThicknessScale { get; set; }

        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }
    }

    public class GalaxySectorConfig
    {
        [YamlMember(Alias = "maxPointsPerChunk")]
        public int MaxPointsPerChunk { get; set; }

        [YamlMember(Alias = "maxSplitDepth")]
        public int MaxSplitDepth { get; set; }

        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }
    }

    public class GalaxySectorChunkConfig
    {
        [YamlMember(Alias = "maxPointsPerBatch")]
        public int MaxPointsPerBatch { get; set; }

        [YamlMember(Alias = "maxSplitDepth")]
        public int MaxSplitDepth { get; set; }

        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }
    }

    public class StarSystemsBatchConfig
    {
        [YamlMember(Alias = "unwrapDistanceMultiplier")]
        public float UnwrapDistanceMultiplier { get; set; }

        [YamlMember(Alias = "octreeSizeMultiplier")]
        public float OctreeSizeMultiplier { get; set; }
    }

    public class StarSystemAsPointConfig
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
