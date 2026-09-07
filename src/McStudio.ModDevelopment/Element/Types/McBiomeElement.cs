using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Biome.java
/// </summary>
public class McBiomeElement : McGeneratableElement
{
    public string? BiomeName { get; set; }

    // Ground blocks
    public MItemBlock? GroundBlock { get; set; }
    public MItemBlock? UndergroundBlock { get; set; }
    public MItemBlock? UnderwaterBlock { get; set; }

    // Colors
    public string? AirColor { get; set; }
    public string? FogColor { get; set; }
    public string? GrassColor { get; set; }
    public string? FoliageColor { get; set; }
    public string? WaterColor { get; set; }
    public string? WaterFogColor { get; set; }
    public string? SkyColor { get; set; }

    // Sounds
    public Sound? AmbientSound { get; set; }
    public Sound? AdditionsSound { get; set; }
    public Sound? Music { get; set; }
    public Sound? MoodSound { get; set; }
    public int MoodSoundDelay { get; set; } = 6000;

    // Particles
    public bool SpawnParticles { get; set; }
    public ParticleEntry? ParticleToSpawn { get; set; }

    // Temperature & precipitation
    public double Temperature { get; set; } = 0.5;
    public string? Precipitation { get; set; } = "rain";
    public double? TemperatureModifier { get; set; }
    public double Downfall { get; set; } = 0.5;

    // Biome type
    public string? BiomeCategory { get; set; } = "plains";
    public bool IsCustom { get; set; }
    public double? Depth { get; set; }
    public double? Scale { get; set; }
    public bool Starlight { get; set; } = true;
    public bool Ceiling { get; set; }

    // Entity spawning
    public List<BiomeSpawnEntry> SpawnEntries { get; set; } = [];

    public McBiomeElement(McModElement? element) : base(element) { }
}

public class BiomeSpawnEntry
{
    public string? EntityType { get; set; }
    public int Weight { get; set; } = 10;
    public int MinGroupSize { get; set; } = 1;
    public int MaxGroupSize { get; set; } = 4;
    public string? SpawnType { get; set; } = "creature";
}