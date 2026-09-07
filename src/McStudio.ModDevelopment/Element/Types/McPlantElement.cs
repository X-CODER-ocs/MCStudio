using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Plant.java
/// </summary>
public class McPlantElement : McGeneratableElement
{
    public int RenderType { get; set; }
    public TextureHolder? Texture { get; set; }
    public string? CustomModelName { get; set; }

    // Plant type
    public string? PlantType { get; set; } = "normal";
    public bool Suspicious { get; set; }
    public bool DoublePlant { get; set; }
    public bool CustomBoneMeal { get; set; }

    // Growing
    public int LightLevel { get; set; } = 9;
    public bool CanBePlacedOn { get; set; }
    public List<MItemBlock> PlaceOnBlocks { get; set; } = [];

    // Bounding box
    public double? BoundingBoxMinX { get; set; }
    public double? BoundingBoxMinY { get; set; }
    public double? BoundingBoxMinZ { get; set; }
    public double? BoundingBoxMaxX { get; set; }
    public double? BoundingBoxMaxY { get; set; }
    public double? BoundingBoxMaxZ { get; set; }

    // Generation
    public bool GenerateFeature { get; set; }
    public string? GenerationType { get; set; }
    public int? GenClusterSize { get; set; } = 8;
    public int? GenMinHeight { get; set; } = 0;
    public int? GenMaxHeight { get; set; } = 256;
    public string? GenDimension { get; set; }
    public List<string> GenBiomes { get; set; } = [];
    public List<string> GenBiomeTags { get; set; } = [];
    public List<string> GenReplaceBlocks { get; set; } = [];

    // Creative tab
    public string? CreativeTab { get; set; }

    // Special info
    public List<string> SpecialInfo { get; set; } = [];

    // Loot table
    public bool UseLootTableForDrops { get; set; }
    public MItemBlock? CustomDrop { get; set; }
    public double? DropAmount { get; set; }

    public McPlantElement(McModElement? element) : base(element) { }
}