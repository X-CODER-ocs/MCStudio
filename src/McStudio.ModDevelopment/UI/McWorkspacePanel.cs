using McStudio.ModDevelopment.Element;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.UI;

/// <summary>
/// Ported from CCS WorkspacePanel.java
/// Logic for the main workspace panel showing element list and details.
/// </summary>
public class McWorkspacePanelLogic : McViewBase
{
    private readonly McWorkspace _workspace;

    public McWorkspacePanelLogic(McWorkspace workspace)
    {
        _workspace = workspace;
    }

    public override string ViewName => "WorkspacePanel";

    public McWorkspace Workspace => _workspace;

    /// <summary>
    /// Get all mod elements, optionally filtered by type
    /// </summary>
    public List<McModElement> GetElements(string? typeFilter = null)
    {
        if (string.IsNullOrEmpty(typeFilter))
            return [.. _workspace.ModElements];
        return _workspace.GetModElementsByType(typeFilter);
    }

    /// <summary>
    /// Create a new mod element of the given type
    /// </summary>
    public McModElement CreateElement(string type, string name)
    {
        var element = new McModElement(_workspace, name, type);
        _workspace.AddModElement(element);
        _workspace.Save();
        ShowStatus($"Created {type}: {name}");
        return element;
    }

    /// <summary>
    /// Delete a mod element
    /// </summary>
    public void DeleteElement(McModElement element)
    {
        _workspace.RemoveModElement(element);
        _workspace.Save();
        ShowStatus($"Deleted: {element.Name}");
    }

    /// <summary>
    /// Rename a mod element
    /// </summary>
    public void RenameElement(McModElement element, string newName)
    {
        var oldName = element.Name;
        element.Name = newName;
        element.RegistryName = McModElement.FromCamelCase(newName);
        _workspace.Save();
        ShowStatus($"Renamed: {oldName} -> {newName}");
    }
}

/// <summary>
/// Ported from the workspace folder/breadcrumb system
/// Manages the folder structure of mod elements.
/// </summary>
public class McWorkspaceFolderSystem
{
    private readonly McWorkspace _workspace;
    private readonly List<McFolder> _folders = [];

    public McWorkspaceFolderSystem(McWorkspace workspace)
    {
        _workspace = workspace;
    }

    public IReadOnlyList<McFolder> Folders => _folders.AsReadOnly();

    public void AddFolder(string name, string? parentPath = null)
    {
        _folders.Add(new McFolder { Name = name, Path = parentPath != null ? $"{parentPath}/{name}" : name });
    }

    public void RemoveFolder(string path)
    {
        _folders.RemoveAll(f => f.Path == path);
    }

    public List<McModElement> GetElementsInFolder(string? folderPath)
    {
        return _workspace.ModElements.Where(e => e.FolderPath == folderPath).ToList();
    }
}

public class McFolder
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string? ParentPath => System.IO.Path.GetDirectoryName(Path);
}