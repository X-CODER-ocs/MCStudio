using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Dimension.java
/// </summary>
public class McDimensionElement : McGeneratableElement
{
    // World gen type
    public string? WorldGenType { get; set; } = "Normal world gen";

    // Blocks
    public MItemBlock? MainFillerBlock { get; set; }
    public MItemBlock? FluidBlock { get; set; }
    public MItemBlock? AirBlock { get; set; }

    // Generation
    public bool GenerateFeature { get; set; }
    public bool GenerateOres { get; set; } = true;
    public bool GenerateStructures { get; set; } = true;
    public bool GenerateLakes { get; set; } = true;
    public bool GenerateAmethyst { get; set; } = true;

    // Height
    public int MinHeight { get; set; } = -64;
    public int MaxHeight { get; set; } = 320;
    public int LogicalHeight { get; set; } = 384;

    // Lighting
    public bool HasSkylight { get; set; } = true;
    public bool HasCeiling { get; set; }
    public bool HasRaids { get; set; } = true;
    public int MonsterSpawnLight { get; set; } = 7;
    public int MonsterSpawnBlockLight { get; set; } = 0;

    // Portal
    public bool HasPortal { get; set; }
    public string? PortalFrame { get; set; }
    public string? PortalTrigger { get; set; }

    // Biomes
    public List<BiomeEntry> BiomesInDimension { get; set; } = [];
    public List<BiomeEntry> BiomesInDimensionCaves { get; set; } = [];

    // Special info
    public List<string> SpecialInfo { get; set; } = [];

    public McDimensionElement(McModElement? element) : base(element) { }
}