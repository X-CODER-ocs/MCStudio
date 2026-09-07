namespace McStudio.ModDevelopment.UI;

/// <summary>
/// Ported from CCS UnregisteredAction.java
/// Represents an action that can be triggered from the UI.
/// </summary>
public class McAction : IComparable<McAction>
{
    public string Name { get; }
    public string? Description { get; set; }
    public string? Tooltip { get; set; }
    public string? IconGlyph { get; set; }
    public bool Enabled { get; set; } = true;
    public Action? ActionCallback { get; set; }
    public Func<bool>? CanExecute { get; set; }

    public McAction(string name, Action? callback = null)
    {
        Name = name;
        ActionCallback = callback;
    }

    public void Execute()
    {
        if (Enabled && (CanExecute?.Invoke() ?? true))
            ActionCallback?.Invoke();
    }

    public int CompareTo(McAction? other)
    {
        if (other == null) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }
}

/// <summary>
/// Ported from CCS action system
/// Manages a registry of actions.
/// </summary>
public class McActionRegistry
{
    private readonly Dictionary<string, McAction> _actions = new();

    public void Register(McAction action)
    {
        _actions[action.Name] = action;
    }

    public McAction? Get(string name)
    {
        return _actions.TryGetValue(name, out var action) ? action : null;
    }

    public IEnumerable<McAction> GetAll() => _actions.Values;

    public void Execute(string name)
    {
        if (_actions.TryGetValue(name, out var action))
            action.Execute();
    }
}

/// <summary>
/// Action categories for organizing UI actions
/// </summary>
public static class McActionCategories
{
    public const string File = "File";
    public const string Edit = "Edit";
    public const string Workspace = "Workspace";
    public const string Element = "Element";
    public const string Generate = "Generate";
    public const string Build = "Build";
    public const string Help = "Help";
}