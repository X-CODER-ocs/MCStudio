using System.Text.Json;

namespace McStudio.ModDevelopment.Workspace;

/// <summary>
/// Ported from CCS WorkspaceFileManager.java
/// Manages file I/O for a Minecraft mod workspace.
/// </summary>
public class McWorkspaceFileManager
{
    private readonly McWorkspace _workspace;

    public McWorkspaceFileManager(McWorkspace workspace)
    {
        _workspace = workspace;
    }

    /// <summary>
    /// Get the workspace folder path
    /// </summary>
    public string GetWorkspaceFolder()
    {
        return Path.GetDirectoryName(_workspace.WorkspacePath) ?? _workspace.WorkspacePath;
    }

    /// <summary>
    /// Get the source folder for Java code
    /// </summary>
    public string GetSourceFolder()
    {
        return Path.Combine(GetWorkspaceFolder(), "src", "main", "java");
    }

    /// <summary>
    /// Get the resources folder
    /// </summary>
    public string GetResourcesFolder()
    {
        return Path.Combine(GetWorkspaceFolder(), "src", "main", "resources");
    }

    /// <summary>
    /// Get the package folder path for Java code
    /// </summary>
    public string GetPackageFolder()
    {
        var packagePath = _workspace.Settings.PackageName.Replace('.', Path.DirectorySeparatorChar);
        return Path.Combine(GetSourceFolder(), packagePath);
    }

    /// <summary>
    /// Get the assets folder for the mod
    /// </summary>
    public string GetAssetsFolder()
    {
        return Path.Combine(GetResourcesFolder(), "assets", _workspace.Settings.ModId);
    }

    /// <summary>
    /// Get the models folder
    /// </summary>
    public string GetModelsFolder()
    {
        return Path.Combine(GetAssetsFolder(), "models");
    }

    /// <summary>
    /// Get the textures folder
    /// </summary>
    public string GetTexturesFolder()
    {
        return Path.Combine(GetAssetsFolder(), "textures");
    }

    /// <summary>
    /// Get the blockstates folder
    /// </summary>
    public string GetBlockStatesFolder()
    {
        return Path.Combine(GetAssetsFolder(), "blockstates");
    }

    /// <summary>
    /// Get the lang folder
    /// </summary>
    public string GetLangFolder()
    {
        return Path.Combine(GetAssetsFolder(), "lang");
    }

    /// <summary>
    /// Get the data folder for the mod
    /// </summary>
    public string GetDataFolder()
    {
        return Path.Combine(GetResourcesFolder(), "data", _workspace.Settings.ModId);
    }

    /// <summary>
    /// Get the loot tables folder
    /// </summary>
    public string GetLootTablesFolder()
    {
        return Path.Combine(GetDataFolder(), "loot_tables", "blocks");
    }

    /// <summary>
    /// Get the recipes folder
    /// </summary>
    public string GetRecipesFolder()
    {
        return Path.Combine(GetDataFolder(), "recipes");
    }

    /// <summary>
    /// Get the tags folder
    /// </summary>
    public string GetTagsFolder()
    {
        return Path.Combine(GetDataFolder(), "tags");
    }

    /// <summary>
    /// Get the advancements folder
    /// </summary>
    public string GetAdvancementsFolder()
    {
        return Path.Combine(GetDataFolder(), "advancements");
    }

    /// <summary>
    /// Create all standard workspace directories
    /// </summary>
    public void CreateStandardDirectories()
    {
        Directory.CreateDirectory(GetSourceFolder());
        Directory.CreateDirectory(GetResourcesFolder());
        Directory.CreateDirectory(GetPackageFolder());
        Directory.CreateDirectory(GetAssetsFolder());
        Directory.CreateDirectory(GetModelsFolder());
        Directory.CreateDirectory(GetModelsFolder("block"));
        Directory.CreateDirectory(GetModelsFolder("item"));
        Directory.CreateDirectory(GetTexturesFolder());
        Directory.CreateDirectory(GetTexturesFolder("blocks"));
        Directory.CreateDirectory(GetTexturesFolder("items"));
        Directory.CreateDirectory(GetTexturesFolder("entities"));
        Directory.CreateDirectory(GetBlockStatesFolder());
        Directory.CreateDirectory(GetLangFolder());
        Directory.CreateDirectory(GetDataFolder());
        Directory.CreateDirectory(GetLootTablesFolder());
        Directory.CreateDirectory(GetRecipesFolder());
        Directory.CreateDirectory(GetTagsFolder());
        Directory.CreateDirectory(GetTagsFolder("blocks"));
        Directory.CreateDirectory(GetTagsFolder("items"));
        Directory.CreateDirectory(GetAdvancementsFolder());
        Directory.CreateDirectory(Path.Combine(GetResourcesFolder(), "META-INF"));
    }

    /// <summary>
    /// Get a subfolder within the models folder
    /// </summary>
    public string GetModelsFolder(string subfolder)
    {
        return Path.Combine(GetModelsFolder(), subfolder);
    }

    /// <summary>
    /// Get a subfolder within the textures folder
    /// </summary>
    public string GetTexturesFolder(string subfolder)
    {
        return Path.Combine(GetTexturesFolder(), subfolder);
    }

    /// <summary>
    /// Get a subfolder within the tags folder
    /// </summary>
    public string GetTagsFolder(string subfolder)
    {
        return Path.Combine(GetTagsFolder(), subfolder);
    }
}