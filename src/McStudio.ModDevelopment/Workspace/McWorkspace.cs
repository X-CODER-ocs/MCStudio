using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McStudio.ModDevelopment.Workspace;

/// <summary>
/// Ported from CCS Workspace.java
/// Represents a Minecraft mod development workspace.
/// </summary>
public class McWorkspace : IDisposable
{
    private static readonly string WorkspaceFileName = "workspace.ccs";

    [JsonInclude] public ObservableCollection<McModElement> ModElements { get; private set; } = [];
    [JsonInclude] public ObservableCollection<McVariableElement> VariableElements { get; private set; } = [];
    [JsonInclude] public ObservableCollection<McSoundElement> SoundElements { get; private set; } = [];
    [JsonInclude] public Dictionary<string, Dictionary<string, string>> LanguageMap { get; private set; } = new()
    {
        ["en_us"] = []
    };

    [JsonInclude] public McWorkspaceSettings Settings { get; set; } = new();

    [JsonInclude] public long CcsVersion { get; set; }

    // Transient fields (not serialized)
    [JsonIgnore] public string WorkspacePath { get; private set; } = string.Empty;
    [JsonIgnore] public bool IsDirty { get; private set; }
    [JsonIgnore] public bool IsRegenerateRequired { get; set; }
    [JsonIgnore] public bool HasFailingGradleDependencies { get; set; }

    public McWorkspace() { }

    public McWorkspace(McWorkspaceSettings settings, string workspacePath)
    {
        Settings = settings;
        WorkspacePath = workspacePath;
    }

    public void MarkDirty()
    {
        IsDirty = true;
    }

    public void MarkClean()
    {
        IsDirty = false;
    }

    public bool ContainsModElement(string name)
    {
        return ModElements.Any(e => e.Name == name);
    }

    public McModElement? GetModElementByName(string name)
    {
        return ModElements.FirstOrDefault(e => e.Name == name);
    }

    public List<McModElement> GetModElementsByType(string type)
    {
        return ModElements.Where(e => e.Type == type).ToList();
    }

    public void AddModElement(McModElement element)
    {
        if (!ModElements.Contains(element))
        {
            element.Reinit(this);
            ModElements.Add(element);
            MarkDirty();
        }
    }

    public void RemoveModElement(McModElement element)
    {
        if (ModElements.Remove(element))
        {
            MarkDirty();
        }
    }

    public void AddVariableElement(McVariableElement element)
    {
        if (!VariableElements.Contains(element))
        {
            VariableElements.Add(element);
            MarkDirty();
        }
    }

    public void AddSoundElement(McSoundElement element)
    {
        if (!SoundElements.Contains(element))
        {
            SoundElements.Add(element);
            MarkDirty();
        }
    }

    public void SetLocalization(string key, string value)
    {
        LanguageMap["en_us"][key] = value;
        foreach (var lang in LanguageMap.Where(l => l.Key != "en_us"))
        {
            lang.Value.TryAdd(key, value);
        }
        MarkDirty();
    }

    public void RemoveLocalizationEntryByKey(string key)
    {
        foreach (var lang in LanguageMap.Values)
        {
            lang.Remove(key);
        }
        MarkDirty();
    }

    public string GetWorkspaceFolder()
    {
        return Path.GetDirectoryName(WorkspacePath) ?? WorkspacePath;
    }

    public void Dispose()
    {
        // Cleanup resources if needed
    }

    public static McWorkspace? ReadFromFile(string workspaceFile)
    {
        if (!File.Exists(workspaceFile))
            return null;

        var json = File.ReadAllText(workspaceFile);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var workspace = JsonSerializer.Deserialize<McWorkspace>(json, options);

        if (workspace != null)
        {
            workspace.WorkspacePath = workspaceFile;
        }

        return workspace;
    }

    public static McWorkspace CreateWorkspace(string workspaceFile, McWorkspaceSettings settings)
    {
        var dir = Path.GetDirectoryName(workspaceFile);
        if (dir != null) Directory.CreateDirectory(dir);

        var workspace = new McWorkspace(settings, workspaceFile);
        workspace.Save();
        return workspace;
    }

    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(WorkspacePath, json);
        MarkClean();
    }

    public void ReloadFromFile()
    {
        var loaded = ReadFromFile(WorkspacePath);
        if (loaded != null)
        {
            ModElements = loaded.ModElements;
            VariableElements = loaded.VariableElements;
            SoundElements = loaded.SoundElements;
            LanguageMap = loaded.LanguageMap;
            Settings = loaded.Settings;
            CcsVersion = loaded.CcsVersion;
        }
    }
}

public class McWorkspaceSettings
{
    public string ModId { get; set; } = "example_mod";
    public string ModName { get; set; } = "Example Mod";
    public string PackageName { get; set; } = "com.example.mod";
    public string MinecraftVersion { get; set; } = "1.21.4";
    public string ModLoader { get; set; } = "neoforge";
    public string Author { get; set; } = "";
    public string Description { get; set; } = "";
    public string CurrentGenerator { get; set; } = "neoforge-1.21.4";
    public string ModElementsPackage { get; set; } = "com.example.mod.init";
}

public class McVariableElement
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "string";
    public string? Value { get; set; }

    public override bool Equals(object? obj) =>
        obj is McVariableElement other && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();
}

public class McSoundElement
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "neutral";
    public List<string> Files { get; set; } = [];

    public override bool Equals(object? obj) =>
        obj is McSoundElement other && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();
}