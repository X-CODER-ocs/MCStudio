using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Fluid.java
/// </summary>
public class McFluidElement : McGeneratableElement
{
    public string? FluidName { get; set; }
    public string? BucketName { get; set; }

    // Textures
    public TextureHolder? TextureStill { get; set; }
    public TextureHolder? TextureFlowing { get; set; }

    // Properties
    public int Luminance { get; set; }
    public int Density { get; set; } = 1000;
    public int Viscosity { get; set; } = 1000;
    public int Temperature { get; set; } = 300;
    public int TickRate { get; set; } = 10;
    public bool Gaseous { get; set; }
    public bool Vaporize { get; set; }
    public bool Flowing { get; set; } = true;

    // Bucket
    public string? CreativeTab { get; set; }
    public string? BucketTexture { get; set; }
    public string? UnlocalizedName { get; set; }

    // Special info
    public List<string> SpecialInfo { get; set; } = [];

    public McFluidElement(McModElement? element) : base(element) { }
}