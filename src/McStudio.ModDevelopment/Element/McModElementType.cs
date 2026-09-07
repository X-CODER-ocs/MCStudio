namespace McStudio.ModDevelopment.Workspace;

/// <summary>
/// Ported from CCS ModElementType.java
/// Defines a type of mod element (block, item, entity, etc.)
/// </summary>
public class McModElementType
{
    public string RegistryName { get; }
    public string ReadableName { get; }
    public string Description { get; }
    public string PluralName { get; }

    private McModElementType(string registryName, string readableName, string description, string pluralName)
    {
        RegistryName = registryName;
        ReadableName = readableName;
        Description = description;
        PluralName = pluralName;
    }

    public static readonly McModElementType Block = new("block", "Block", "A basic block", "blocks");
    public static readonly McModElementType Item = new("item", "Item", "A basic item", "items");
    public static readonly McModElementType LivingEntity = new("livingentity", "Living Entity", "A living entity with AI", "livingentities");
    public static readonly McModElementType Procedure = new("procedure", "Procedure", "A visual procedure", "procedures");
    public static readonly McModElementType Recipe = new("recipe", "Recipe", "A crafting recipe", "recipes");
    public static readonly McModElementType CreativeTab = new("creativetab", "Creative Tab", "A creative mode tab", "creativetabs");
    public static readonly McModElementType Dimension = new("dimension", "Dimension", "A custom dimension", "dimensions");
    public static readonly McModElementType Biome = new("biome", "Biome", "A custom biome", "biomes");
    public static readonly McModElementType Unknown = new("unknown", "Unknown", "Unknown element type", "unknown");

    private static readonly Dictionary<string, McModElementType> Registry = new()
    {
        ["block"] = Block,
        ["item"] = Item,
        ["livingentity"] = LivingEntity,
        ["procedure"] = Procedure,
        ["recipe"] = Recipe,
        ["creativetab"] = CreativeTab,
        ["dimension"] = Dimension,
        ["biome"] = Biome,
    };

    public static McModElementType Get(string registryName)
    {
        return Registry.TryGetValue(registryName.ToLowerInvariant(), out var type) ? type : Unknown;
    }

    public static IEnumerable<McModElementType> GetAll() => Registry.Values;

    public override string ToString() => ReadableName;
    public override bool Equals(object? obj) => obj is McModElementType other && RegistryName == other.RegistryName;
    public override int GetHashCode() => RegistryName.GetHashCode();
}