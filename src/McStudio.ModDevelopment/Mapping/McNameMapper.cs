using System.Text.RegularExpressions;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Mapping;

/// <summary>
/// Ported from CCS NameMapper.java
/// Maps element names to Minecraft resource names.
/// </summary>
public partial class McNameMapper
{
    public const string UnknownElement = "deleted_mod_element";
    public const string CcsPrefix = "CUSTOM:";
    public const string ExternalPrefix = "EXTERNAL:";

    private readonly string _mappingSource;
    private McWorkspace? _workspace;

    public McNameMapper(McWorkspace workspace, string mappingSource)
    {
        _workspace = workspace;
        _mappingSource = mappingSource;
    }

    public McWorkspace? Workspace
    {
        get => _workspace;
        set => _workspace = value;
    }

    public string MappingSource => _mappingSource;

    /// <summary>
    /// Map a name to a Minecraft resource name
    /// </summary>
    public string Map(string name)
    {
        if (string.IsNullOrEmpty(name))
            return UnknownElement;

        if (name.StartsWith(ExternalPrefix))
            return name[ExternalPrefix.Length..];

        if (name.StartsWith(CcsPrefix))
        {
            var elementName = name[CcsPrefix.Length..];
            if (_workspace != null)
            {
                var element = _workspace.GetModElementByName(elementName);
                if (element != null)
                    return $"{_workspace.Settings.ModId}:{element.GetRegistryName()}";
            }
            return $"{_workspace?.Settings.ModId ?? "unknown"}:{McRegistryNameFixer.FromCamelCase(elementName)}";
        }

        return name; // vanilla minecraft item
    }

    /// <summary>
    /// Check if a string contains a custom element reference
    /// </summary>
    public static bool HasCustomElement(string value)
    {
        return value.Contains(CcsPrefix) || value.Contains(ExternalPrefix);
    }

    /// <summary>
    /// Extract the element name from a CUSTOM: reference
    /// </summary>
    public static string? GetCustomElementName(string value)
    {
        if (value.StartsWith(CcsPrefix))
            return value[CcsPrefix.Length..];
        if (value.StartsWith(ExternalPrefix))
            return value[ExternalPrefix.Length..];
        return null;
    }
}

/// <summary>
/// Ported from CCS RegistryNameFixer.java
/// </summary>
public partial class McRegistryNameFixer
{
    [GeneratedRegex("[^a-z0-9/._-]+")]
    private static partial Regex RegistryNamePattern();

    public static string Fix(string original)
    {
        if (string.IsNullOrEmpty(original)) return original;
        return RegistryNamePattern().Replace(
            Transliterate(original).ToLowerInvariant().Replace(" ", "_"), "");
    }

    public static string FromCamelCase(string original)
    {
        if (string.IsNullOrEmpty(original)) return original;
        var retval = Transliterate(original.Replace(" ", "_"));
        retval = CamelToSnake(retval);
        retval = retval.ToLowerInvariant();
        retval = retval.Replace("__", "_");
        return RegistryNamePattern().Replace(retval, "");
    }

    private static string CamelToSnake(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input,
            "([a-z])([A-Z])", "$1_$2");
    }

    private static string Transliterate(string input)
    {
        // Simple ASCII transliteration for non-ASCII characters
        var result = new System.Text.StringBuilder();
        foreach (var c in input)
        {
            if (c < 128)
                result.Append(c);
            else
            {
                // Map common Unicode characters to ASCII
                result.Append(c switch
                {
                    '\u00e4' or '\u00e0' or '\u00e1' or '\u00e2' or '\u00e3' or '\u00e5' => 'a',
                    '\u00e7' => 'c',
                    '\u00e8' or '\u00e9' or '\u00ea' or '\u00eb' => 'e',
                    '\u00ec' or '\u00ed' or '\u00ee' or '\u00ef' => 'i',
                    '\u00f1' => 'n',
                    '\u00f2' or '\u00f3' or '\u00f4' or '\u00f5' or '\u00f6' => 'o',
                    '\u00f9' or '\u00fa' or '\u00fb' or '\u00fc' => 'u',
                    '\u00fd' or '\u00ff' => 'y',
                    '\u00df' => 's',
                    ' ' => '_',
                    _ => '_'
                });
            }
        }
        return result.ToString();
    }
}

/// <summary>
/// Ported from CCS MappableElement.java
/// </summary>
public class McMappableElement
{
    public string ElementName { get; }
    public string Type { get; }
    public string? RegistryName { get; }

    public McMappableElement(string elementName, string type, string? registryName = null)
    {
        ElementName = elementName;
        Type = type;
        RegistryName = registryName;
    }
}

/// <summary>
/// Ported from CCS MappingLoader.java
/// Loads mappings from JSON files
/// </summary>
public class McMappingLoader
{
    private readonly Dictionary<string, Dictionary<string, string>> _mappings = new();

    /// <summary>
    /// Load mappings from a JSON file
    /// </summary>
    public void LoadMappings(string filePath)
    {
        if (!File.Exists(filePath)) return;

        var json = File.ReadAllText(filePath);
        var mappings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
        if (mappings != null)
        {
            foreach (var kvp in mappings)
                _mappings[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    /// Get a mapping value
    /// </summary>
    public string? GetMapping(string category, string key)
    {
        if (_mappings.TryGetValue(category, out var cat) && cat.TryGetValue(key, out var value))
            return value;
        return null;
    }
}