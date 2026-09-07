using System.Text.Json.Serialization;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element;

/// <summary>
/// Ported from CCS Block.java (element/types/Block.java)
/// Represents a Minecraft block mod element with all its properties.
/// </summary>
public class McBlockElement : McGeneratableElement
{
    // Texture references
    public string? Texture { get; set; }
    public string? TextureTop { get; set; }
    public string? TextureLeft { get; set; }
    public string? TextureFront { get; set; }
    public string? TextureRight { get; set; }
    public string? TextureBack { get; set; }

    // Rendering
    public int RenderType { get; set; }
    public string CustomModelName { get; set; } = "";
    public int RotationMode { get; set; }
    public bool EnablePitch { get; set; }
    public bool EmissiveRendering { get; set; }
    public bool DisplayFluidOverlay { get; set; }

    // Properties
    public string? BlockBase { get; set; }
    public string? PlantType { get; set; }
    public string? PlantTypeInRecipe { get; set; }

    // Creative tab
    public string? CreativeTab { get; set; }
    public string? DestroyTool { get; set; }
    public bool RequiresCorrectTool { get; set; }
    public int? CustomDrop { get; set; }
    public double? DropAmount { get; set; }
    public bool? UseLootTableForDrops { get; set; }

    // Hardness and resistance
    public double Hardness { get; set; } = 1.0;
    public double Resistance { get; set; } = 10.0;

    // Light
    public int Luminance { get; set; }
    public bool IsTransparent { get; set; }
    public int LightOpacity { get; set; } = 15;

    // Redstone
    public bool IsPowered { get; set; }
    public bool IsRedstoneConductor { get; set; } = true;
    public bool EnableRedstone { get; set; }
    public int? TickRate { get; set; }

    // Bounding box
    public double? BoundingBoxMinX { get; set; }
    public double? BoundingBoxMinY { get; set; }
    public double? BoundingBoxMinZ { get; set; }
    public double? BoundingBoxMaxX { get; set; }
    public double? BoundingBoxMaxY { get; set; }
    public double? BoundingBoxMaxZ { get; set; }

    // Piston behavior
    public bool IsPistonOut { get; set; }

    // Generation
    public bool GenerateFeature { get; set; }
    public string? GenerationType { get; set; }
    public int? GenClusterSize { get; set; }
    public int? GenMinHeight { get; set; }
    public int? GenMaxHeight { get; set; }
    public string? GenDimension { get; set; }
    public List<string> GenBiomes { get; set; } = [];
    public List<string> GenBiomeTags { get; set; } = [];
    public List<string> GenReplaceBlocks { get; set; } = [];

    // Special info
    public List<string> SpecialInfo { get; set; } = [];

    // Color
    public string? BlockMapColor { get; set; }

    // Name provider
    public string? UnlocalizedName { get; set; }

    // Slab
    public bool IsDoubleSlab { get; set; }
    public bool IsFullBlock { get; set; } = true;

    // Note block
    public string? NoteBlockInstrument { get; set; }

    public McBlockElement(McModElement? element) : base(element) { }
}