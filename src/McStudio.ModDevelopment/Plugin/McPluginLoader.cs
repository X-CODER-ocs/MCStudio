using System.Text.Json;

namespace McStudio.ModDevelopment.Plugin;

/// <summary>
/// Ported from CCS PluginLoader.java
/// Detects and loads all builtin or custom plugins.
/// </summary>
public class McPluginLoader : IDisposable
{
    private static readonly string PluginManifestFile = "plugin.json";

    public static McPluginLoader? Instance { get; private set; }

    public static void InitInstance()
    {
        Instance = new McPluginLoader();
    }

    private readonly List<McPlugin> _plugins = [];
    private readonly List<McPlugin> _javaPlugins = [];
    private readonly List<McPluginLoadFailure> _failedPlugins = [];
    private readonly List<McPluginUpdateInfo> _pluginUpdates = [];

    public IReadOnlyList<McPlugin> Plugins => _plugins.AsReadOnly();
    public IReadOnlyList<McPluginLoadFailure> FailedPlugins => _failedPlugins.AsReadOnly();
    public IReadOnlyList<McPluginUpdateInfo> PluginUpdates => _pluginUpdates.AsReadOnly();

    /// <summary>
    /// Load all plugins from the specified directories
    /// </summary>
    public void LoadAllPlugins(string pluginsDir, string builtinPluginsDir)
    {
        // Load builtin plugins first
        if (Directory.Exists(builtinPluginsDir))
            LoadPluginsFromDir(builtinPluginsDir, true);

        // Load user plugins
        if (Directory.Exists(pluginsDir))
            LoadPluginsFromDir(pluginsDir, false);

        // Sort by weight (higher weight = earlier)
        _plugins.Sort();
    }

    private void LoadPluginsFromDir(string dir, bool builtin)
    {
        foreach (var pluginDir in Directory.GetDirectories(dir))
        {
            var manifestPath = Path.Combine(pluginDir, PluginManifestFile);
            if (!File.Exists(manifestPath))
                continue;

            try
            {
                var json = File.ReadAllText(manifestPath);
                var manifest = JsonSerializer.Deserialize<McPluginManifest>(json);
                if (manifest == null || string.IsNullOrEmpty(manifest.Id))
                    continue;

                var plugin = new McPlugin
                {
                    Id = manifest.Id,
                    FilePath = pluginDir,
                    IsBuiltin = builtin,
                    Weight = manifest.Weight,
                    SupportedVersions = manifest.SupportedVersions,
                    Info = new McPluginInfo
                    {
                        Name = manifest.Name ?? manifest.Id,
                        Description = manifest.Description ?? "",
                        Author = manifest.Author ?? "",
                        Version = manifest.Version ?? "1.0.0",
                        Dependencies = manifest.Dependencies ?? [],
                    },
                    JavaPluginClass = manifest.JavaPlugin,
                };

                _plugins.Add(plugin);
            }
            catch (Exception ex)
            {
                _failedPlugins.Add(new McPluginLoadFailure
                {
                    Id = Path.GetFileName(pluginDir),
                    FailureReason = ex.Message,
                    FilePath = pluginDir
                });
            }
        }
    }

    /// <summary>
    /// Get a plugin by its ID
    /// </summary>
    public McPlugin? GetPlugin(string id)
    {
        return _plugins.Find(p => p.Id == id);
    }

    /// <summary>
    /// Get all plugins of a specific type
    /// </summary>
    public List<McPlugin> GetPluginsWithJavaPlugin()
    {
        return _plugins.Where(p => p.JavaPluginClass != null).ToList();
    }

    public void Dispose()
    {
        _plugins.Clear();
        _javaPlugins.Clear();
        _failedPlugins.Clear();
        _pluginUpdates.Clear();
    }
}

/// <summary>
/// JSON manifest structure for a plugin
/// </summary>
public class McPluginManifest
{
    public string Id { get; set; } = "";
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? Version { get; set; }
    public int Weight { get; set; }
    public List<string>? Dependencies { get; set; }
    public List<long>? SupportedVersions { get; set; }
    public string? JavaPlugin { get; set; }
    public string? UpdateJsonUrl { get; set; }
}