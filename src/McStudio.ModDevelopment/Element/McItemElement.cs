using System.Text.Json.Serialization;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element;

/// <summary>
/// Ported from CCS Item.java (element/types/Item.java)
/// Represents a Minecraft item mod element.
/// </summary>
public class McItemElement : McGeneratableElement
{
    public string? Texture { get; set; }
    public string? RenderType { get; set; }
    public string? CustomModelName { get; set; }
    public string? CreativeTab { get; set; }
    public int StackSize { get; set; } = 64;
    public int? Enchantability { get; set; }
    public bool? HasGlow { get; set; }
    public bool? IsFood { get; set; }
    public int? Nutrition { get; set; }
    public double? SaturationMod { get; set; }
    public bool? AlwaysEdible { get; set; }
    public bool? IsMeat { get; set; }
    public bool? IsFastFood { get; set; }
    public string? ItemUseAnimation { get; set; }
    public int? UseDuration { get; set; }
    public int? DamageVsEntity { get; set; }
    public string? DestroyTool { get; set; }
    public bool? RequiresCorrectTool { get; set; }
    public List<string> SpecialInfo { get; set; } = [];
    public string? UnlocalizedName { get; set; }

    public McItemElement(McModElement? element) : base(element) { }
}