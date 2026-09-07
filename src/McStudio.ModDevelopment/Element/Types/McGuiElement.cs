using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS GUI.java
/// </summary>
public class McGuiElement : McGeneratableElement
{
    public int Type { get; set; }
    public int Width { get; set; } = 176;
    public int Height { get; set; } = 166;
    public int InventoryOffsetX { get; set; }
    public int InventoryOffsetY { get; set; }
    public bool RenderBgLayer { get; set; } = true;
    public bool DoesPauseGame { get; set; }

    public List<GUIComponent> Components { get; set; } = [];

    public Procedure? OnOpen { get; set; }
    public Procedure? OnTick { get; set; }
    public Procedure? OnClosed { get; set; }

    public McGuiElement(McModElement? element) : base(element) { }
}