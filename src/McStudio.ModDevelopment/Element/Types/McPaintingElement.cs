using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Painting.java
/// Represents a custom painting.
/// </summary>
public class McPaintingElement : McGeneratableElement
{
    public string? PaintingName { get; set; }
    public int Width { get; set; } = 1;
    public int Height { get; set; } = 1;
    public string? Texture { get; set; }

    public McPaintingElement(McModElement? element) : base(element) { }
}