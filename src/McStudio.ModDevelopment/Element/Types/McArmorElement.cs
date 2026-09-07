using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Armor.java
/// </summary>
public class McArmorElement : McGeneratableElement
{
    public bool EnableHelmet { get; set; }
    public TextureHolder? TextureHelmet { get; set; }
    public bool EnableBody { get; set; }
    public TextureHolder? TextureBody { get; set; }
    public bool EnableLeggings { get; set; }
    public TextureHolder? TextureLeggings { get; set; }
    public bool EnableBoots { get; set; }
    public TextureHolder? TextureBoots { get; set; }

    // Armor properties
    public double HelmetDamageReduction { get; set; } = 2;
    public double BodyDamageReduction { get; set; } = 5;
    public double LeggingsDamageReduction { get; set; } = 4;
    public double BootsDamageReduction { get; set; } = 1;
    public int ArmorToughness { get; set; }
    public int KnockbackResistance { get; set; }
    public int Enchantability { get; set; } = 9;
    public int EquipSound { get; set; } = 4;

    // Repair
    public MItemBlock? RepairItem { get; set; }
    public string? CreativeTab { get; set; }

    public McArmorElement(McModElement? element) : base(element) { }
}