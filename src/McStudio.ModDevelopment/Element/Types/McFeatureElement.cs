using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Feature.java
/// </summary>
public class McFeatureElement : McGeneratableElement
{
    public bool SkipPlacement { get; set; }
    public GenerationStep? GenerationStep { get; set; }
    public List<BiomeEntry> RestrictionBiomes { get; set; } = [];
    public Procedure? GenerateCondition { get; set; }
    public string? FeatureXml { get; set; }

    public McFeatureElement(McModElement? element) : base(element) { }
}