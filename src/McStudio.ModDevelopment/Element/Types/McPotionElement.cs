using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Potion.java
/// </summary>
public class McPotionElement : McGeneratableElement
{
    public string? PotionName { get; set; }
    public string? SplashName { get; set; }
    public string? LingeringName { get; set; }
    public string? ArrowName { get; set; }
    public List<CustomEffectEntry> Effects { get; set; } = [];

    public McPotionElement(McModElement? element) : base(element) { }
}

public class CustomEffectEntry
{
    public EffectEntry? Effect { get; set; }
    public int Duration { get; set; } = 100;
    public bool Infinite { get; set; }
    public int Amplifier { get; set; }
    public bool Ambient { get; set; }
    public bool ShowParticles { get; set; } = true;
    public bool ShowIcon { get; set; } = true;
}