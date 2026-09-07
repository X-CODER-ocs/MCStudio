using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Tab.java
/// Represents a creative mode tab.
/// </summary>
public class McTabElement : McGeneratableElement
{
    public string? TabName { get; set; }
    public TextureHolder? Icon { get; set; }
    public string? IconItem { get; set; }
    public bool ShowSearch { get; set; } = true;
    public bool ShowScrollbar { get; set; } = true;
    public string? LabelColor { get; set; }
    public string? BackgroundColor { get; set; }

    public McTabElement(McModElement? element) : base(element) { }
}