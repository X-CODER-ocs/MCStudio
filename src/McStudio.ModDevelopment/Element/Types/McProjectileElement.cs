using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Projectile.java
/// </summary>
public class McProjectileElement : McGeneratableElement
{
    public TextureHolder? Texture { get; set; }
    public bool IsThrowable { get; set; } = true;
    public bool IsArrow { get; set; }
    public bool IsPotion { get; set; }

    // Damage
    public double Damage { get; set; } = 5.0;
    public bool Knockback { get; set; }
    public bool FireProjectile { get; set; }
    public int FireDuration { get; set; } = 5;

    // Gravity
    public double Gravity { get; set; } = 0.03;
    public double Velocity { get; set; } = 1.5;
    public double Inaccuracy { get; set; } = 1.0;

    // Particles
    public string? ParticleTrail { get; set; }

    // On hit
    public Procedure? OnHitEntity { get; set; }
    public Procedure? OnHitBlock { get; set; }

    public McProjectileElement(McModElement? element) : base(element) { }
}