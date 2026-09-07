using System.Text.Json;

namespace McStudio.ModDevelopment.Generator;

/// <summary>
/// Ported from CCS GeneratorTemplate.java
/// Represents a single template file to be generated.
/// </summary>
public class McGeneratorTemplate
{
    public string FilePath { get; }
    public string TemplateIdentifier { get; }
    public Dictionary<string, object> TemplateDefinition { get; }

    public McGeneratorTemplate(string filePath, string templateIdentifier, Dictionary<string, object> templateDefinition)
    {
        FilePath = filePath;
        TemplateIdentifier = templateIdentifier;
        TemplateDefinition = templateDefinition;
    }

    public static McGeneratorTemplate FromFile(string filePath, Dictionary<string, object> templateDefinition)
    {
        var identifier = Path.GetFileName(filePath);
        return new McGeneratorTemplate(filePath, identifier, templateDefinition);
    }

    public bool IsHidden()
    {
        return TemplateDefinition.TryGetValue("hidden", out var val) && val?.ToString() == "true";
    }

    public string? GetUsercodeComment()
    {
        return TemplateDefinition.GetValueOrDefault("usercode_comment")?.ToString();
    }

    public string? GetTemplateFile()
    {
        return TemplateDefinition.GetValueOrDefault("template")?.ToString();
    }

    public string? GetWriter()
    {
        return TemplateDefinition.GetValueOrDefault("writer")?.ToString();
    }

    public string? GetVariableName()
    {
        return TemplateDefinition.GetValueOrDefault("variable_name")?.ToString();
    }

    public string? GetName()
    {
        return TemplateDefinition.GetValueOrDefault("name")?.ToString();
    }
}

/// <summary>
/// Ported from CCS GeneratorFile.java
/// Represents a generated file with its content and writer type.
/// </summary>
public class McGeneratorFile
{
    public McGeneratorTemplate Source { get; }
    public WriterType Writer { get; }
    public string Contents { get; }

    public McGeneratorFile(McGeneratorTemplate source, WriterType writer, string contents)
    {
        Source = source;
        Writer = writer;
        Contents = contents;
    }

    public string FilePath => Source.FilePath;
    public string? UsercodeComment => Source.GetUsercodeComment();

    public enum WriterType
    {
        Java,
        Json,
        File,
        Js
    }

    public static WriterType FromString(string? str, string filePath)
    {
        if (string.IsNullOrEmpty(str))
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".java" => WriterType.Java,
                ".json" => WriterType.Json,
                ".js" => WriterType.Js,
                _ => WriterType.File
            };
        }

        return str.ToLowerInvariant() switch
        {
            "java" => WriterType.Java,
            "json" => WriterType.Json,
            "js" => WriterType.Js,
            "file" => WriterType.File,
            _ => WriterType.File
        };
    }
}