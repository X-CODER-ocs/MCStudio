namespace McStudio.ModDevelopment.Plugin;

/// <summary>
/// Ported from CCS Plugin.java
/// A Plugin is a mod for MC Studio allowing to alter, improve or extend features.
/// </summary>
public class McPlugin : IComparable<McPlugin>
{
    public string Id { get; set; } = "";
    public string FilePath { get; set; } = "";
    public bool IsBuiltin { get; set; }
    public string? LoadFailure { get; set; }
    public int Weight { get; set; }
    public List<long>? SupportedVersions { get; set; }
    public McPluginInfo? Info { get; set; }
    public string? JavaPluginClass { get; set; }

    public bool IsLoaded => LoadFailure == null;

    public int CompareTo(McPlugin? other)
    {
        if (other == null) return 1;
        return -Weight.CompareTo(other.Weight); // higher weight = earlier
    }
}

/// <summary>
/// Ported from CCS PluginInfo.java
/// Contains metadata about a plugin.
/// </summary>
public class McPluginInfo
{
    public const string VersionNotSpecified = "not specified";

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Author { get; set; } = "";
    public string Credits { get; set; } = "";
    public string Version { get; set; } = VersionNotSpecified;
    public List<string> Dependencies { get; set; } = [];
    public string? UpdateJsonUrl { get; set; }
    public int PluginPageId { get; set; }
}

/// <summary>
/// Ported from CCS PluginLoadFailure.java
/// </summary>
public class McPluginLoadFailure
{
    public string Id { get; set; } = "";
    public string FailureReason { get; set; } = "";
    public string? FilePath { get; set; }
}

/// <summary>
/// Ported from CCS PluginUpdateInfo.java
/// </summary>
public class McPluginUpdateInfo
{
    public string PluginId { get; set; } = "";
    public string? UpdateVersion { get; set; }
    public string? UpdateUrl { get; set; }
}