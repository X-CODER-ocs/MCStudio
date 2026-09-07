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
    public static readonly McModElementType Armor = new("armor", "Armor", "A set of armor", "armors");
    public static readonly McModElementType Tool = new("tool", "Tool", "A tool or weapon", "tools");
    public static readonly McModElementType Feature = new("feature", "Feature", "A world generation feature", "features");
    public static readonly McModElementType Fluid = new("fluid", "Fluid", "A custom fluid", "fluids");
    public static readonly McModElementType Gui = new("gui", "GUI", "A graphical user interface", "guis");
    public static readonly McModElementType Plant = new("plant", "Plant", "A plant or crop", "plants");
    public static readonly McModElementType Potion = new("potion", "Potion", "A custom potion effect", "potions");
    public static readonly McModElementType Projectile = new("projectile", "Projectile", "A throwable projectile", "projectiles");
    public static readonly McModElementType Enchantment = new("enchantment", "Enchantment", "A custom enchantment", "enchantments");
    public static readonly McModElementType KeyBinding = new("keybinding", "Key Binding", "A custom key binding", "keybindings");
    public static readonly McModElementType LootTable = new("loottable", "Loot Table", "A custom loot table", "loottables");
    public static readonly McModElementType Overlay = new("overlay", "Overlay", "A custom HUD overlay", "overlays");
    public static readonly McModElementType Painting = new("painting", "Painting", "A custom painting", "paintings");
    public static readonly McModElementType Structure = new("structure", "Structure", "A world structure", "structures");
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
        ["armor"] = Armor,
        ["tool"] = Tool,
        ["feature"] = Feature,
        ["fluid"] = Fluid,
        ["gui"] = Gui,
        ["plant"] = Plant,
        ["potion"] = Potion,
        ["projectile"] = Projectile,
        ["enchantment"] = Enchantment,
        ["keybinding"] = KeyBinding,
        ["loottable"] = LootTable,
        ["overlay"] = Overlay,
        ["painting"] = Painting,
        ["structure"] = Structure,
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