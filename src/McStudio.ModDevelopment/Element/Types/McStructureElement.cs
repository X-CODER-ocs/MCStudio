using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Structure.java
/// </summary>
public class McStructureElement : McGeneratableElement
{
    public string? StructureFile { get; set; }
    public string? Projection { get; set; } = "rigid";
    public List<MItemBlock> IgnoredBlocks { get; set; } = [];

    public int Spacing { get; set; } = 5;
    public int Separation { get; set; } = 2;

    public List<BiomeEntry> RestrictionBiomes { get; set; } = [];
    public string? TerrainAdaptation { get; set; } = "none";
    public GenerationStep? GenerationStep { get; set; }
    public string? SurfaceDetectionType { get; set; } = "WORLD_SURFACE_WG";

    public McStructureElement(McModElement? element) : base(element) { }
}