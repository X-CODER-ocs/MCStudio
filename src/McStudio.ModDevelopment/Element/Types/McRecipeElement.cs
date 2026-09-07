using McStudio.ModDevelopment.Element.Parts;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Element.Types;

/// <summary>
/// Ported from CCS Recipe.java
/// </summary>
public class McRecipeElement : McGeneratableElement
{
    public string? RecipeType { get; set; } = "Crafting";
    public int RecipeRetstackSize { get; set; } = 1;
    public string? Group { get; set; }
    public List<MItemBlock> UnlockingItems { get; set; } = [];

    // Cooking
    public string? CookingBookCategory { get; set; } = "MISC";
    public double XpReward { get; set; } = 1.0;
    public int CookingTime { get; set; } = 200;

    // Crafting
    public string? CraftingBookCategory { get; set; } = "MISC";
    public bool RecipeShapeless { get; set; }
    public MItemBlock[]? RecipeSlots { get; set; }
    public MItemBlock? RecipeReturnStack { get; set; }

    // Smelting
    public MItemBlock? SmeltingInputStack { get; set; }
    public MItemBlock? SmeltingReturnStack { get; set; }

    // Blasting
    public MItemBlock? BlastingInputStack { get; set; }
    public MItemBlock? BlastingReturnStack { get; set; }

    // Smoking
    public MItemBlock? SmokingInputStack { get; set; }
    public MItemBlock? SmokingReturnStack { get; set; }

    // Campfire
    public MItemBlock? CampfireInputStack { get; set; }
    public MItemBlock? CampfireReturnStack { get; set; }

    // Stone cutting
    public MItemBlock? StoneCuttingInputStack { get; set; }
    public MItemBlock? StoneCuttingReturnStack { get; set; }

    // Smithing
    public MItemBlock? SmithingTemplate { get; set; }
    public MItemBlock? SmithingInputBase { get; set; }
    public MItemBlock? SmithingInputAddition { get; set; }
    public MItemBlock? SmithingReturnStack { get; set; }

    // Brewing
    public MItemBlock? BrewingInput { get; set; }
    public MItemBlock? BrewingIngredient { get; set; }
    public MItemBlock? BrewingReturnStack { get; set; }

    public McRecipeElement(McModElement? element) : base(element) { }
}