using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS KeyBinding.java
/// </summary>
public class McKeyBindingElement : McGeneratableElement
{
    public string? KeyBindingName { get; set; }
    public string? KeyCode { get; set; } = "KEY_X";
    public string? Category { get; set; } = "misc";
    public Procedure? OnKeyPressed { get; set; }
    public Procedure? OnKeyReleased { get; set; }

    public McKeyBindingElement(McModElement? element) : base(element) { }
}