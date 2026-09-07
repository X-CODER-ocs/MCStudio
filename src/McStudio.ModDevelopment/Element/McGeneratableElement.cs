using System.Text.Json.Serialization;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element;

/// <summary>
/// Ported from CCS GeneratableElement.java
/// Abstract base class for all mod element types that can generate code.
/// </summary>
public abstract class McGeneratableElement
{
    public const int FormatVersion = 88;

    [JsonIgnore]
    public McModElement? ModElement { get; set; }

    [JsonIgnore]
    public bool ConversionApplied { get; set; }

    protected McGeneratableElement(McModElement? element)
    {
        ModElement = element;
    }

    /// <summary>
    /// Override to provide additional template data
    /// </summary>
    public virtual Dictionary<string, object>? GetAdditionalTemplateData() => null;

    public virtual bool IsUnknown() => false;

    /// <summary>
    /// Unknown element type - used when the original type no longer exists
    /// </summary>
    public class Unknown : McGeneratableElement
    {
        public Unknown(McModElement? element) : base(element) { }
        public override bool IsUnknown() => true;
    }
}