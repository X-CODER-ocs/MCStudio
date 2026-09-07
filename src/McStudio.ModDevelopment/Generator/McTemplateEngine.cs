namespace McStudio.ModDevelopment.Generator;

/// <summary>
/// Simple template engine for mod code generation.
/// Ported from CCS TemplateGenerator.java (FreeMarker-based).
/// Uses a simple string replacement approach for now.
/// </summary>
public class McTemplateEngine
{
    /// <summary>
    /// Process a template string with the given data model
    /// </summary>
    public string ProcessTemplate(string template, Dictionary<string, object> dataModel)
    {
        var result = template;

        foreach (var entry in dataModel)
        {
            var placeholder = $"${{{entry.Key}}}";
            result = result.Replace(placeholder, entry.Value?.ToString() ?? "");
        }

        return result;
    }

    /// <summary>
    /// Load a template from a file
    /// </summary>
    public string LoadTemplate(string templatePath)
    {
        if (File.Exists(templatePath))
            return File.ReadAllText(templatePath);
        return "";
    }
}