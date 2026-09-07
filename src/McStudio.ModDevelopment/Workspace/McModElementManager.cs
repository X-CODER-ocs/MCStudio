using System.Text.Json;
using McStudio.ModDevelopment.Element;

namespace McStudio.ModDevelopment.Workspace;

/// <summary>
/// Ported from CCS ModElementManager.java
/// Manages mod element storage, loading, and saving.
/// </summary>
public class McModElementManager
{
    private readonly McWorkspace _workspace;

    public McModElementManager(McWorkspace workspace)
    {
        _workspace = workspace;
    }

    /// <summary>
    /// Get the directory where mod element definitions are stored
    /// </summary>
    public string GetElementsDirectory()
    {
        return Path.Combine(_workspace.GetWorkspaceFolder(), "elements");
    }

    /// <summary>
    /// Save a mod element's definition to disk
    /// </summary>
    public void SaveModElement(McModElement element)
    {
        var elementsDir = GetElementsDirectory();
        Directory.CreateDirectory(elementsDir);

        var filePath = Path.Combine(elementsDir, $"{element.Name}.json");
        var json = JsonSerializer.Serialize(new
        {
            _fv = McGeneratableElement.FormatVersion,
            _type = element.Type,
            definition = new Dictionary<string, object>
            {
                ["name"] = element.Name,
                ["type"] = element.Type,
                ["registry_name"] = element.GetRegistryName(),
            }
        }, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Load a mod element from disk
    /// </summary>
    public McModElement? LoadModElement(string name)
    {
        var elementsDir = GetElementsDirectory();
        var filePath = Path.Combine(elementsDir, $"{name}.json");

        if (!File.Exists(filePath))
            return null;

        var json = File.ReadAllText(filePath);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var type = root.GetProperty("_type").GetString() ?? "block";
        var element = new McModElement(_workspace, name, type);

        return element;
    }

    /// <summary>
    /// Load all mod elements from disk
    /// </summary>
    public List<McModElement> LoadAllModElements()
    {
        var elements = new List<McModElement>();
        var elementsDir = GetElementsDirectory();

        if (!Directory.Exists(elementsDir))
            return elements;

        foreach (var file in Directory.GetFiles(elementsDir, "*.json"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var element = LoadModElement(name);
            if (element != null)
                elements.Add(element);
        }

        return elements;
    }

    /// <summary>
    /// Delete a mod element from disk
    /// </summary>
    public void DeleteModElement(McModElement element)
    {
        var elementsDir = GetElementsDirectory();
        var filePath = Path.Combine(elementsDir, $"{element.Name}.json");

        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    /// <summary>
    /// Rename a mod element
    /// </summary>
    public void RenameModElement(McModElement element, string newName)
    {
        DeleteModElement(element);
        element.Name = newName;
        element.RegistryName = McModElement.FromCamelCase(newName);
        SaveModElement(element);
    }
}