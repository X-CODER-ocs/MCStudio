using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using McStudio.ModDevelopment.Generator;
using McStudio.ModDevelopment.Gradle;
using McStudio.ModDevelopment.Workspace;
using SulfurLauncher.Module.DefaultPage;
using Tio.Avalonia.Standard.Tab.Entries;
using Tio.Avalonia.Standard.Tab.Interface;

namespace SulfurLauncher.Views.Pages;

[DefaultPage("pages_mcstudio_mod")]
public partial class McStudioModPage : UserControl, ITioTabPage
{
    private readonly ObservableCollection<McWorkspace> _workspaces = [];
    private McWorkspace? _selectedWorkspace;

    public McStudioModPage()
    {
        InitializeComponent();
        DataContext = this;
        LoadWorkspaces();
    }

    public PageInfo PageInfo { get; init; } = new()
    {
        Title = "[M] 模组开发",
        IconGlyph = "M",
        IconFont = null
    };

    public TabEntry HostTab { get; set; }

    public ObservableCollection<McWorkspace> Workspaces => _workspaces;

    public void OnClose()
    {
        DataContext = null;
    }

    private void LoadWorkspaces()
    {
        var workspaceDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "MCStudio", "workspaces");

        if (Directory.Exists(workspaceDir))
        {
            foreach (var file in Directory.GetFiles(workspaceDir, "*.ccs", SearchOption.AllDirectories))
            {
                var ws = McWorkspace.ReadFromFile(file);
                if (ws != null)
                    _workspaces.Add(ws);
            }
        }
    }

    private void OnNewWorkspace(object? sender, RoutedEventArgs e)
    {
        var workspaceDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "MCStudio", "workspaces");

        var modId = $"mod_{DateTime.Now:yyyyMMddHHmmss}";
        var workspaceFile = Path.Combine(workspaceDir, modId, $"{modId}.ccs");

        var settings = new McWorkspaceSettings
        {
            ModId = modId,
            ModName = "My Mod",
            PackageName = $"com.example.{modId}",
            MinecraftVersion = "1.21.4",
            ModLoader = "neoforge",
            Author = "Player",
            Description = "A Minecraft mod created with MC Studio"
        };

        var workspace = McWorkspace.CreateWorkspace(workspaceFile, settings);
        _workspaces.Add(workspace);
        SelectWorkspace(workspace);
    }

    private void OnWorkspaceClick(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border { DataContext: McWorkspace ws })
            SelectWorkspace(ws);
    }

    private void SelectWorkspace(McWorkspace ws)
    {
        _selectedWorkspace = ws;
        SelectedWorkspaceTitle.Text = ws.Settings.ModId;
        ElementList.ItemsSource = ws.ModElements;
        ElementListTitle.Text = $"元素列表 ({ws.ModElements.Count})";
    }

    private void OnAddElement(object? sender, RoutedEventArgs e)
    {
        if (_selectedWorkspace == null) return;
        if (sender is Button { Tag: string tag })
            AddElement(tag);
    }

    private void AddElement(string type)
    {
        if (_selectedWorkspace == null) return;

        var elementName = $"{type}_{DateTime.Now:HHmmss}";
        var element = new McModElement(_selectedWorkspace, elementName, type);
        _selectedWorkspace.AddModElement(element);
        _selectedWorkspace.Save();
        ElementList.ItemsSource = _selectedWorkspace.ModElements;
        ElementListTitle.Text = $"元素列表 ({_selectedWorkspace.ModElements.Count})";
    }

    private void OnGenerateCode(object? sender, RoutedEventArgs e)
    {
        if (_selectedWorkspace == null) return;

        using var generator = new McCodeGenerator(_selectedWorkspace);
        generator.GenerateBase();
        foreach (var element in _selectedWorkspace.ModElements)
            generator.GenerateElement(element);
    }

    private async void OnBuildMod(object? sender, RoutedEventArgs e)
    {
        OnGenerateCode(sender, e);
        if (_selectedWorkspace == null) return;

        var runner = new McGradleRunner(_selectedWorkspace.GetWorkspaceFolder());
        if (!runner.HasGradleWrapper())
        {
            // Try to generate Gradle wrapper
            return;
        }

        var result = await runner.Build();
        if (result.IsSuccess)
            Console.WriteLine("Build successful!");
        else
            Console.WriteLine($"Build failed: {result.Error}");
    }
}