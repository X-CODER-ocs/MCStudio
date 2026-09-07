using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Enchantment.java
/// </summary>
public class McEnchantmentElement : McGeneratableElement
{
    public string? EnchantmentName { get; set; }
    public string? EnchantmentType { get; set; } = "weapon";
    public int MinLevel { get; set; } = 1;
    public int MaxLevel { get; set; } = 5;
    public int MinCostBase { get; set; } = 1;
    public int MinCostPerLevel { get; set; } = 10;
    public int MaxCostBase { get; set; } = 5;
    public int MaxCostPerLevel { get; set; } = 10;
    public bool IsCurse { get; set; }
    public bool IsTreasure { get; set; }
    public bool IsTradeable { get; set; } = true;
    public bool IsDiscoverable { get; set; } = true;
    public List<string> CompatibleItems { get; set; } = [];

    public McEnchantmentElement(McModElement? element) : base(element) { }
}