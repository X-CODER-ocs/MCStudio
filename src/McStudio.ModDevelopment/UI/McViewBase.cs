using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStudio.ModDevelopment.UI;

/// <summary>
/// Ported from CCS ViewBase.java
/// Base class for all views/panels.
/// </summary>
public abstract class McViewBase : INotifyPropertyChanged, IDisposable
{
    private bool _isActive;
    private string? _statusMessage;

    public bool IsActive
    {
        get => _isActive;
        set { _isActive = value; OnPropertyChanged(); }
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public abstract string ViewName { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void OnActivated() { }
    public virtual void OnDeactivated() { }
    public virtual void Dispose() { }

    /// <summary>
    /// Show a status message in the status bar
    /// </summary>
    protected void ShowStatus(string message)
    {
        StatusMessage = message;
    }
}

/// <summary>
/// Ported from CCS ViewBase.java
/// Base class for views that can be opened in tabs.
/// </summary>
public abstract class McTabView : McViewBase
{
    public abstract string TabTitle { get; }
    public abstract string? TabIcon { get; }
    public bool CanClose { get; set; } = true;

    public virtual void OnTabSelected() { }
    public virtual void OnTabDeselected() { }
    public virtual bool OnTabClosing() => true;
}