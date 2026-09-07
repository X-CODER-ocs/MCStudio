using McStudio.ModDevelopment.Element;
using McStudio.ModDevelopment.Workspace;

namespace McStudio.ModDevelopment.UI;

/// <summary>
/// Ported from CCS ModElementGUI.java
/// Abstract base class for editing a mod element of a specific type.
/// </summary>
public abstract class McModElementEditorGui : McTabView
{
    private bool _isDirty;
    private bool _isListening;

    protected McModElementEditorGui(McWorkspace workspace, McModElement element)
    {
        Workspace = workspace;
        ModElement = element;
    }

    public McWorkspace Workspace { get; }
    public McModElement ModElement { get; }
    public override string ViewName => $"Edit_{ModElement.Name}";
    public override string TabTitle => $"{ModElement.Name} [{ModElement.Type}]";

    public bool IsDirty
    {
        get => _isDirty;
        protected set { _isDirty = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Called when a field changes to mark the editor as dirty
    /// </summary>
    protected void MarkDirty()
    {
        if (_isListening)
            IsDirty = true;
    }

    /// <summary>
    /// Start listening for changes
    /// </summary>
    protected void StartListening()
    {
        _isListening = true;
    }

    /// <summary>
    /// Stop listening for changes
    /// </summary>
    protected void StopListening()
    {
        _isListening = false;
    }

    /// <summary>
    /// Save the current element state
    /// </summary>
    public virtual void Save()
    {
        Workspace.Save();
        IsDirty = false;
    }

    /// <summary>
    /// Validate the current element state
    /// </summary>
    public virtual ValidationResult Validate()
    {
        var result = new ValidationResult();
        if (string.IsNullOrWhiteSpace(ModElement.Name))
            result.AddError("Element name cannot be empty");
        if (ModElement.Name.Length > 64)
            result.AddError("Element name is too long (max 64 characters)");
        return result;
    }

    /// <summary>
    /// Generate code for this element
    /// </summary>
    public virtual bool GenerateCode()
    {
        using var generator = new Generator.McCodeGenerator(Workspace);
        return generator.GenerateElement(ModElement);
    }

    public override bool OnTabClosing()
    {
        if (IsDirty)
            Save();
        return true;
    }
}

/// <summary>
/// Validation result
/// </summary>
public class ValidationResult
{
    public List<string> Errors { get; } = [];
    public List<string> Warnings { get; } = [];

    public bool IsValid => Errors.Count == 0;

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);

    public override string ToString()
    {
        if (IsValid && Warnings.Count == 0) return "Valid";
        var parts = new List<string>();
        if (Errors.Count > 0) parts.Add($"Errors: {string.Join(", ", Errors)}");
        if (Warnings.Count > 0) parts.Add($"Warnings: {string.Join(", ", Warnings)}");
        return string.Join("; ", parts);
    }
}