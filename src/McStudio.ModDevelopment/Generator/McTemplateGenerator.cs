using System.Text;
using System.Text.Json;
using McStudio.ModDevelopment.Element;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.Generator;

/// <summary>
/// Ported from CCS TemplateGenerator.java
/// Generates code from templates using a data model.
/// </summary>
public class McTemplateGenerator
{
    private readonly McCodeGenerator _generator;
    private readonly McBaseDataModelProvider _baseDataModelProvider;
    private readonly Dictionary<string, string> _templateCache = new();

    public McTemplateGenerator(McCodeGenerator generator)
    {
        _generator = generator;
        _baseDataModelProvider = new McBaseDataModelProvider(generator.Workspace, generator.GeneratorName);
    }

    /// <summary>
    /// Generate base mod files from a template
    /// </summary>
    public string GenerateBaseFromTemplate(string templateName, Dictionary<string, object> dataModel)
    {
        // Add base data model
        foreach (var kvp in _baseDataModelProvider.Provide())
            dataModel[kvp.Key] = kvp.Value;

        dataModel["variables"] = _generator.Workspace.VariableElements.ToList();
        dataModel["sounds"] = _generator.Workspace.SoundElements.ToList();

        return GenerateTemplate(templateName, dataModel);
    }

    /// <summary>
    /// Generate code for a specific element from a template
    /// </summary>
    public string GenerateElementFromTemplate(McGeneratableElement element, string templateName,
        Dictionary<string, object> dataModel, Dictionary<string, object>? additionalData = null)
    {
        // Add base data model
        foreach (var kvp in _baseDataModelProvider.Provide())
            dataModel[kvp.Key] = kvp.Value;

        dataModel["data"] = element;
        if (element.ModElement != null)
        {
            dataModel["registryname"] = element.ModElement.GetRegistryName();
            dataModel["REGISTRYNAME"] = element.ModElement.GetRegistryNameUpper();
            dataModel["name"] = element.ModElement.Name;
        }

        if (additionalData != null)
        {
            foreach (var kvp in additionalData)
                dataModel[kvp.Key] = kvp.Value;
        }

        return GenerateTemplate(templateName, dataModel);
    }

    /// <summary>
    /// Generate a template using string replacement
    /// </summary>
    private string GenerateTemplate(string templateName, Dictionary<string, object> dataModel)
    {
        // Load the template
        var template = LoadTemplate(templateName);
        if (string.IsNullOrEmpty(template))
            return "";

        // Simple string replacement based template engine
        var result = template;

        foreach (var entry in dataModel)
        {
            var placeholder = $"${{{entry.Key}}}";
            var value = entry.Value?.ToString() ?? "";
            result = result.Replace(placeholder, value);
        }

        return result;
    }

    /// <summary>
    /// Load a template from the file system or cache
    /// </summary>
    private string LoadTemplate(string templateName)
    {
        if (_templateCache.TryGetValue(templateName, out var cached))
            return cached;

        // Try to find the template file
        var templatePaths = new[]
        {
            Path.Combine(_generator.GeneratorName, "templates", templateName),
            Path.Combine("templates", templateName),
            templateName
        };

        foreach (var path in templatePaths)
        {
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                _templateCache[templateName] = content;
                return content;
            }
        }

        return "";
    }
}

/// <summary>
/// Ported from CCS BaseDataModelProvider.java
/// Provides base data model entries for template generation.
/// </summary>
public class McBaseDataModelProvider
{
    private readonly McWorkspace _workspace;
    private readonly string _generatorName;

    public McBaseDataModelProvider(McWorkspace workspace, string generatorName)
    {
        _workspace = workspace;
        _generatorName = generatorName;
    }

    public Dictionary<string, object> Provide()
    {
        var settings = _workspace.Settings;
        return new Dictionary<string, object>
        {
            ["modid"] = settings.ModId,
            ["modname"] = settings.ModName,
            ["package"] = settings.PackageName,
            ["author"] = settings.Author,
            ["description"] = settings.Description,
            ["mcversion"] = settings.MinecraftVersion,
            ["modloader"] = settings.ModLoader,
            ["workspace"] = _workspace,
            ["generator"] = _generatorName,
            ["elements_package"] = settings.ModElementsPackage,
        };
    }
}