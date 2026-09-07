using System.Reflection;
using System.Text;
using Scriban;
using Scriban.Runtime;

namespace McStudio.ModDevelopment.Generator;

/// <summary>
/// Scriban-based template engine that replaces the FreeMarker FTL engine from CCS.
/// Loads templates from embedded resources and renders them with a data model.
/// </summary>
public class McScribanTemplateEngine : IDisposable
{
    private static readonly Assembly Assembly = typeof(McScribanTemplateEngine).Assembly;
    private readonly Dictionary<string, ScribanTemplate> _templateCache = new();
    private readonly string _generatorName;

    public McScribanTemplateEngine(string generatorName)
    {
        _generatorName = generatorName;
    }

    public string GeneratorName => _generatorName;

    /// <summary>
    /// Render a template with the given data model
    /// </summary>
    public string Render(string templateName, Dictionary<string, object> dataModel)
    {
        var template = LoadTemplate(templateName);
        if (template == null)
            return $"// Template not found: {templateName}";

        var context = CreateTemplateContext(dataModel);
        return template.Render(context);
    }

    /// <summary>
    /// Load a Scriban template from embedded resources
    /// </summary>
    private ScribanTemplate? LoadTemplate(string templateName)
    {
        // Check cache
        if (_templateCache.TryGetValue(templateName, out var cached))
            return cached;

        // Try to find the template in embedded resources
        var resourceName = $"McStudio.ModDevelopment.Templates.{_generatorName}.{templateName}";
        try
        {
            using var stream = Assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var content = reader.ReadToEnd();
                var template = ScribanTemplate.Parse(content);
                _templateCache[templateName] = template;
                return template;
            }
        }
        catch
        {
            // Ignore - try other paths
        }

        // Try without generator prefix
        resourceName = templateName.Replace("/", ".");
        try
        {
            using var stream = Assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var content = reader.ReadToEnd();
                var template = ScribanTemplate.Parse(content);
                _templateCache[templateName] = template;
                return template;
            }
        }
        catch
        {
            // Ignore
        }

        return null;
    }

    /// <summary>
    /// Create a Scriban template context with the data model
    /// </summary>
    private TemplateContext CreateTemplateContext(Dictionary<string, object> dataModel)
    {
        var scriptObject = new ScriptObject();
        foreach (var kvp in dataModel)
            scriptObject[kvp.Key] = kvp.Value;

        // Add built-in functions
        scriptObject["escape_string"] = new Func<string?, string>(EscapeString);
        scriptObject["camel_to_snake"] = new Func<string, string>(CamelToSnake);
        scriptObject["upper_first"] = new Func<string, string>(UpperFirst);
        scriptObject["lower_first"] = new Func<string, string>(LowerFirst);
        scriptObject["java_package"] = new Func<string, string>(ToJavaPackage);

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        return context;
    }

    /// <summary>
    /// Escape a string for Java code
    /// </summary>
    private static string EscapeString(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "\"\"";
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }

    private static string CamelToSnake(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
                sb.Append('_');
            sb.Append(char.ToLowerInvariant(input[i]));
        }
        return sb.ToString();
    }

    private static string UpperFirst(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToUpperInvariant(input[0]) + input[1..];
    }

    private static string LowerFirst(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    private static string ToJavaPackage(string packageName)
    {
        return packageName.Replace('.', '/');
    }

    public void Dispose()
    {
        _templateCache.Clear();
    }
}

/// <summary>
/// Simple wrapper around Scriban Template
/// </summary>
public class ScribanTemplate
{
    private readonly Template _template;

    private ScribanTemplate(Template template)
    {
        _template = template;
    }

    public static ScribanTemplate Parse(string content)
    {
        return new ScribanTemplate(Template.Parse(content));
    }

    public string Render(TemplateContext context)
    {
        return _template.Render(context);
    }
}