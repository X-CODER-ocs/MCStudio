using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS LootTable.java
/// </summary>
public class McLootTableElement : McGeneratableElement
{
    public string? LootTableName { get; set; }
    public string? PoolType { get; set; } = "chest";
    public int Rolls { get; set; } = 1;
    public bool BonusRolls { get; set; }
    public List<LootEntry> Entries { get; set; } = [];

    public McLootTableElement(McModElement? element) : base(element) { }
}

public class LootEntry
{
    public string? Item { get; set; }
    public int Weight { get; set; } = 1;
    public int MinCount { get; set; } = 1;
    public int MaxCount { get; set; } = 1;
    public double? Chance { get; set; }
    public string? Function { get; set; }
    public Dictionary<string, object>? Functions { get; set; }
}