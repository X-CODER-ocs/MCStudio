using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Overlay.java
/// Represents a custom HUD overlay.
/// </summary>
public class McOverlayElement : McGeneratableElement
{
    public string? OverlayName { get; set; }
    public int Priority { get; set; } = 100;
    public bool OverlayTarget { get; set; } = true;
    public bool OverlayTargetCrosshair { get; set; }
    public Procedure? DisplayCondition { get; set; }
    public Procedure? RenderCode { get; set; }

    public McOverlayElement(McModElement? element) : base(element) { }
}