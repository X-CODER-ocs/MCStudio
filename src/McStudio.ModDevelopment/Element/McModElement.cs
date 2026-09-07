using System.Text.Json.Serialization;
using McStudio.ModDevelopment.Element;
using McStudio.ModDevelopment.Element.Types;

namespace McStudio.ModDevelopment.Workspace;

/// <summary>
/// Ported from CCS ModElement.java
/// Represents a single mod element (block, item, entity, etc.) in the workspace.
/// </summary>
public class McModElement : IEquatable<McModElement>
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "block";
    public bool Compiles { get; set; } = true;
    public bool IsCodeLocked { get; set; }
    public string? RegistryName { get; set; }
    public string? FolderPath { get; set; }

    [JsonInclude]
    public Dictionary<string, object?> Metadata { get; private set; } = [];

    // Transient (not serialized)
    [JsonIgnore] public McWorkspace? Workspace { get; private set; }

    public McModElement() { }

    public McModElement(McWorkspace workspace, string name, string type)
    {
        Name = name;
        Type = type;
        RegistryName = FromCamelCase(name);
        Reinit(workspace);
    }

    public void Reinit(McWorkspace workspace)
    {
        Workspace = workspace;
    }

    public void PutMetadata(string key, object? data)
    {
        Metadata[key] = data;
    }

    public object? GetMetadata(string key)
    {
        return Metadata.GetValueOrDefault(key);
    }

    public List<string> GetAssociatedFiles()
    {
        if (Metadata.GetValueOrDefault("files") is List<object> fileList)
            return fileList.OfType<string>().ToList();
        return [];
    }

    public void SetAssociatedFiles(List<string> files)
    {
        Metadata["files"] = files.Cast<object>().ToList();
    }

    public string GetRegistryName()
    {
        return RegistryName ?? FromCamelCase(Name);
    }

    public string GetRegistryNameUpper()
    {
        return GetRegistryName().ToUpperInvariant();
    }

    public void SetParentFolder(string? path)
    {
        FolderPath = path;
    }

    /// <summary>
    /// Get the generatable element data for this mod element.
    /// Ported from CCS ModElement.getGeneratableElement()
    /// </summary>
    public McGeneratableElement? GetGeneratableElement()
    {
        if (Metadata.TryGetValue("generatable_data", out var data) && data is McGeneratableElement genElement)
            return genElement;

        // Create appropriate generatable element based on type
        McGeneratableElement? result = Type switch
        {
            "block" => new McBlockElement(this),
            "item" => new McItemElement(this),
            "livingentity" => new McLivingEntityElement(this),
            "armor" => new McArmorElement(this),
            "tool" => new McToolElement(this),
            "biome" => new McBiomeElement(this),
            "dimension" => new McDimensionElement(this),
            "feature" => new McFeatureElement(this),
            "fluid" => new McFluidElement(this),
            "gui" => new McGuiElement(this),
            "plant" => new McPlantElement(this),
            "potion" => new McPotionElement(this),
            "procedure" => new McProcedureElement(this),
            "projectile" => new McProjectileElement(this),
            "enchantment" => new McEnchantmentElement(this),
            "keybinding" => new McKeyBindingElement(this),
            "loottable" => new McLootTableElement(this),
            "overlay" => new McOverlayElement(this),
            "painting" => new McPaintingElement(this),
            "structure" => new McStructureElement(this),
            "recipe" => new McRecipeElement(this),
            "creativetab" => new McTabElement(this),
            _ => null
        };

        if (result != null)
            Metadata["generatable_data"] = result;
        return result;
    }

    public override string ToString() => Name;

    public bool Equals(McModElement? other) =>
        other is not null && Name == other.Name;

    public override bool Equals(object? obj) =>
        Equals(obj as McModElement);

    public override int GetHashCode() => Name.GetHashCode();

    /// <summary>
    /// Converts a CamelCase name to a snake_case registry name.
    /// Ported from CCS RegistryNameFixer.fromCamelCase()
    /// </summary>
    public static string FromCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i > 0)
                result.Append('_');
            result.Append(char.ToLowerInvariant(name[i]));
        }
        return result.ToString();
    }
}