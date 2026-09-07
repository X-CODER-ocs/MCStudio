using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using McStudio.ModDevelopment.Element;
using McStudio.ModDevelopment.Element.Types;
using McStudio.ModDevelopment.Generator;
using McStudio.ModDevelopment.Gradle;
using McStudio.ModDevelopment.UI;
using McStudio.ModDevelopment.Workspace;

namespace SulfurLauncher.Views.Pages;

/// <summary>
/// ViewModel for the workspace editor.
/// Ported from CCS WorkspacePanel.java + ModElementGUI.java
/// </summary>
public class McStudioWorkspaceViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly McWorkspace _workspace;
    private readonly McWorkspacePanelLogic _panelLogic;
    private McModElement? _selectedElement;
    private string _statusMessage = "";

    public McStudioWorkspaceViewModel(McWorkspace workspace)
    {
        _workspace = workspace;
        _panelLogic = new McWorkspacePanelLogic(workspace);
        RefreshElements();
    }

    public McWorkspace Workspace => _workspace;

    public ObservableCollection<McModElement> Elements { get; } = [];

    public ObservableCollection<McModElementType> AvailableTypes { get; } =
    [
        McModElementType.Block,
        McModElementType.Item,
        McModElementType.LivingEntity,
        McModElementType.Armor,
        McModElementType.Tool,
        McModElementType.Recipe,
        McModElementType.Procedure,
        McModElementType.Dimension,
        McModElementType.Biome,
        McModElementType.Fluid,
        McModElementType.Plant,
        McModElementType.Feature,
        McModElementType.Gui,
        McModElementType.Potion,
        McModElementType.Projectile,
        McModElementType.Structure,
        McModElementType.Enchantment,
        McModElementType.KeyBinding,
        McModElementType.LootTable,
        McModElementType.Overlay,
        McModElementType.Painting,
        McModElementType.CreativeTab,
    ];

    public McModElement? SelectedElement
    {
        get => _selectedElement;
        set
        {
            _selectedElement = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(SelectedElementType));
            OnPropertyChanged(nameof(SelectedElementRegistryName));
        }
    }

    public bool HasSelection => _selectedElement != null;
    public string? SelectedElementType => _selectedElement?.Type;
    public string? SelectedElementRegistryName => _selectedElement?.GetRegistryName();

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public string WorkspaceName => _workspace.Settings.ModId;
    public string WorkspaceModId => _workspace.Settings.ModId;
    public string MinecraftVersion => _workspace.Settings.MinecraftVersion;
    public string ModLoader => _workspace.Settings.ModLoader;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void RefreshElements()
    {
        Elements.Clear();
        foreach (var e in _workspace.ModElements)
            Elements.Add(e);
    }

    /// <summary>
    /// Create a new element of the given type
    /// </summary>
    public McModElement CreateElement(string type, string? name = null)
    {
        var elementName = name ?? $"{type}_{DateTime.Now:HHmmss}";
        var element = _panelLogic.CreateElement(type, elementName);
        RefreshElements();
        StatusMessage = $"Created {type}: {elementName}";
        return element;
    }

    /// <summary>
    /// Delete the selected element
    /// </summary>
    public void DeleteSelectedElement()
    {
        if (_selectedElement == null) return;
        var name = _selectedElement.Name;
        _panelLogic.DeleteElement(_selectedElement);
        SelectedElement = null;
        RefreshElements();
        StatusMessage = $"Deleted: {name}";
    }

    /// <summary>
    /// Rename the selected element
    /// </summary>
    public void RenameSelectedElement(string newName)
    {
        if (_selectedElement == null) return;
        _panelLogic.RenameElement(_selectedElement, newName);
        RefreshElements();
        StatusMessage = $"Renamed to: {newName}";
    }

    /// <summary>
    /// Generate all code
    /// </summary>
    public void GenerateAllCode()
    {
        using var generator = new McCodeGenerator(_workspace);
        generator.GenerateBase();
        foreach (var element in _workspace.ModElements)
            generator.GenerateElement(element);
        StatusMessage = "Code generation complete";
    }

    /// <summary>
    /// Build the mod
    /// </summary>
    public async Task<bool> BuildMod()
    {
        GenerateAllCode();
        var runner = new McGradleRunner(_workspace.GetWorkspaceFolder());
        if (!runner.HasGradleWrapper())
        {
            StatusMessage = "No Gradle wrapper found - run 'gradle wrapper' first";
            return false;
        }

        StatusMessage = "Building mod...";
        var result = await runner.Build();
        StatusMessage = result.IsSuccess
            ? "Build successful!"
            : $"Build failed: {result.Error[..Math.Min(result.Error.Length, 200)]}";
        return result.IsSuccess;
    }

    /// <summary>
    /// Get the element type display name
    /// </summary>
    public static string GetElementTypeDisplayName(string type)
    {
        return McModElementType.Get(type).ReadableName;
    }

    public void Dispose()
    {
        _panelLogic.Dispose();
    }
}