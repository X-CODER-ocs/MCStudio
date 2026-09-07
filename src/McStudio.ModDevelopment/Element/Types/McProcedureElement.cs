using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Procedure.java
/// A visual procedure using Blockly blocks.
/// </summary>
public class McProcedureElement : McGeneratableElement
{
    public string? ProcedureXml { get; set; }
    public string? Trigger { get; set; } = "no_ext_trigger";
    public List<string> Dependencies { get; set; } = [];
    public bool HasReturn { get; set; }

    public McProcedureElement(McModElement? element) : base(element) { }
}