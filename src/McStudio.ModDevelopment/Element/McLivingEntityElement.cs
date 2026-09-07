using System.Text.Json.Serialization;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element;

/// <summary>
/// Ported from CCS LivingEntity.java (element/types/LivingEntity.java)
/// Represents a Minecraft living entity mod element.
/// </summary>
public class McLivingEntityElement : McGeneratableElement
{
    // Entity properties
    public string? MobName { get; set; }
    public string? CreativeTab { get; set; }
    public double Health { get; set; } = 20;
    public double MovementSpeed { get; set; } = 0.3;
    public double AttackDamage { get; set; } = 3;
    public double? ArmorBaseValue { get; set; }
    public double? KnockbackResistance { get; set; }
    public double? FollowRange { get; set; }

    // Spawning
    public bool Spawnable { get; set; } = true;
    public bool SpawnInDungeons { get; set; }
    public int MinSpawn { get; set; } = 1;
    public int MaxSpawn { get; set; } = 4;
    public string? SpawnType { get; set; } = "monster";
    public int? SpawnWeight { get; set; } = 10;
    public List<string> SpawnBiomes { get; set; } = [];
    public List<string> SpawnBiomeTags { get; set; } = [];

    // Model
    public string? Model { get; set; }
    public string? Texture { get; set; }
    public string? TextureGlow { get; set; }
    public double? ModelWidth { get; set; } = 0.6;
    public double? ModelHeight { get; set; } = 1.8;
    public double? ScaleX { get; set; } = 1;
    public double? ScaleY { get; set; } = 1;
    public double? ScaleZ { get; set; } = 1;
    public double? ScaleAge { get; set; }

    // AI
    public bool HasAI { get; set; } = true;
    public List<string> AIParams { get; set; } = [];

    // Experience
    public int? MinXp { get; set; }
    public int? MaxXp { get; set; }

    // Equipment
    public string? MainHand { get; set; }
    public string? OffHand { get; set; }
    public string? Helmet { get; set; }
    public string? Chestplate { get; set; }
    public string? Leggings { get; set; }
    public string? Boots { get; set; }

    public McLivingEntityElement(McModElement? element) : base(element) { }
}