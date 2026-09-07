using System.Text.Json;

namespace McStudio.ModDevelopment.Generator;

/// <summary>
/// Ported from CCS GeneratorConfiguration.java
/// Represents the configuration of a code generator for a specific Minecraft version/mod loader.
/// </summary>
public class McGeneratorConfiguration : IComparable<McGeneratorConfiguration>
{
    public string GeneratorName { get; }
    public McGeneratorFlavor Flavor { get; }
    public string MinecraftVersion { get; }
    public int SortOrder { get; set; }
    public Dictionary<string, object>? Config { get; set; }

    public McGeneratorConfiguration(string generatorName, McGeneratorFlavor flavor, string minecraftVersion)
    {
        GeneratorName = generatorName;
        Flavor = flavor;
        MinecraftVersion = minecraftVersion;
    }

    public bool SupportsElementType(string elementType)
    {
        if (Config == null) return true;
        if (Config.TryGetValue("supported_element_types", out var val) && val is List<object> types)
            return types.Contains(elementType);
        return true;
    }

    public int CompareTo(McGeneratorConfiguration? other)
    {
        if (other == null) return 1;
        return SortOrder.CompareTo(other.SortOrder);
    }

    public static McGeneratorConfiguration? GetRecommendedForFlavor(
        List<McGeneratorConfiguration> configurations, McGeneratorFlavor flavor)
    {
        return configurations
            .Where(c => c.Flavor.Equals(flavor))
            .OrderBy(c => c.SortOrder)
            .FirstOrDefault();
    }

    public static McGeneratorConfiguration? GetRecommendedForBaseLanguage(
        List<McGeneratorConfiguration> configurations, McGeneratorFlavor.BaseLanguage baseLanguage)
    {
        return configurations
            .Where(c => c.Flavor.Language == baseLanguage)
            .OrderBy(c => c.SortOrder)
            .FirstOrDefault();
    }

    public override string ToString() => $"{GeneratorName} ({Flavor.Name})";
}

/// <summary>
/// Ported from CCS GeneratorFlavor.java
/// Enum-like class representing the mod loader / platform flavor.
/// </summary>
public class McGeneratorFlavor : IEquatable<McGeneratorFlavor>
{
    public string Name { get; }
    public GamePlatform Platform { get; }
    public BaseLanguage Language { get; }
    public bool IsEnabled { get; }

    private McGeneratorFlavor(string name, GamePlatform platform, BaseLanguage language, bool enabled = true)
    {
        Name = name;
        Platform = platform;
        Language = language;
        IsEnabled = enabled;
    }

    public enum GamePlatform { JAVAEDITION, BEDROCKEDITION, UNKNOWN }
    public enum BaseLanguage { JAVA, JSON, UNKNOWN }

    // Official flavors
    public static readonly McGeneratorFlavor FORGE = new("forge", GamePlatform.JAVAEDITION, BaseLanguage.JAVA);
    public static readonly McGeneratorFlavor FABRIC = new("fabric", GamePlatform.JAVAEDITION, BaseLanguage.JAVA);
    public static readonly McGeneratorFlavor NEOFORGE = new("neoforge", GamePlatform.JAVAEDITION, BaseLanguage.JAVA);
    public static readonly McGeneratorFlavor SPIGOT = new("spigot", GamePlatform.JAVAEDITION, BaseLanguage.JAVA);
    public static readonly McGeneratorFlavor PAPER = new("paper", GamePlatform.JAVAEDITION, BaseLanguage.JAVA);
    public static readonly McGeneratorFlavor QUILT = new("quilt", GamePlatform.JAVAEDITION, BaseLanguage.JAVA);
    public static readonly McGeneratorFlavor DATAPACK = new("datapack", GamePlatform.JAVAEDITION, BaseLanguage.JSON, false);
    public static readonly McGeneratorFlavor RESOURCEPACK = new("resourcepack", GamePlatform.JAVAEDITION, BaseLanguage.JSON, false);
    public static readonly McGeneratorFlavor ADDON = new("addon", GamePlatform.BEDROCKEDITION, BaseLanguage.JSON, false);
    public static readonly McGeneratorFlavor UNKNOWN = new("unknown", GamePlatform.UNKNOWN, BaseLanguage.UNKNOWN, false);

    public static readonly List<McGeneratorFlavor> OfficialFlavors = [FORGE, DATAPACK, RESOURCEPACK, ADDON, NEOFORGE];

    public static McGeneratorFlavor FromString(string name)
    {
        return name.ToLowerInvariant() switch
        {
            "forge" => FORGE,
            "fabric" => FABRIC,
            "neoforge" => NEOFORGE,
            "spigot" => SPIGOT,
            "paper" => PAPER,
            "quilt" => QUILT,
            "datapack" => DATAPACK,
            "resourcepack" => RESOURCEPACK,
            "addon" => ADDON,
            _ => UNKNOWN
        };
    }

    public bool Equals(McGeneratorFlavor? other)
    {
        if (other is null) return false;
        return Name == other.Name && Platform == other.Platform;
    }

    public override bool Equals(object? obj) => Equals(obj as McGeneratorFlavor);
    public override int GetHashCode() => HashCode.Combine(Name, Platform);
    public override string ToString() => Name;
}