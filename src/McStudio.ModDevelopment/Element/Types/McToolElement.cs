using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Tool.java
/// </summary>
public class McToolElement : McGeneratableElement
{
    public string? ToolType { get; set; } = "Pickaxe";
    public int RenderType { get; set; }
    public int BlockingRenderType { get; set; }
    public TextureHolder? Texture { get; set; }
    public string? CustomModelName { get; set; }
    public string? BlockingModelName { get; set; }
    public TextureHolder? GuiTexture { get; set; }

    // Tool properties
    public double Efficiency { get; set; } = 4.0;
    public double AttackDamage { get; set; } = 1.0;
    public double AttackSpeed { get; set; } = 1.2;
    public int Enchantability { get; set; } = 10;
    public int HarvestLevel { get; set; } = 1;
    public int Durability { get; set; } = 250;
    public double Reach { get; set; }

    // Repair
    public MItemBlock? RepairItem { get; set; }

    // Damage
    public bool IsMelee { get; set; } = true;
    public bool IsRanged { get; set; }
    public double RangedDamage { get; set; }
    public double RangedSpeed { get; set; } = 1.0;

    // Creative tab
    public string? CreativeTab { get; set; }

    // Special info
    public List<string> SpecialInfo { get; set; } = [];

    // Attributes
    public List<AttributeModifierEntry> Attributes { get; set; } = [];

    public McToolElement(McModElement? element) : base(element) { }
}