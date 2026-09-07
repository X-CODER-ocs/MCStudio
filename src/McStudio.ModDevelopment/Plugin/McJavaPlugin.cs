namespace McStudio.ModDevelopment.Plugin;

/// <summary>
/// Ported from CCS JavaPlugin.java
/// Base class for plugins that contain Java code executed at runtime.
/// </summary>
public abstract class McJavaPlugin
{
    public McPlugin? Plugin { get; set; }

    /// <summary>
    /// Called when the plugin is loaded
    /// </summary>
    public virtual void OnLoad() { }

    /// <summary>
    /// Called when the plugin is unloaded
    /// </summary>
    public virtual void OnUnload() { }

    /// <summary>
    /// Called when the application is fully loaded
    /// </summary>
    public virtual void OnApplicationLoaded() { }
}

/// <summary>
/// Ported from CCS MCREvent.java
/// Base event class for the event system
/// </summary>
public abstract class McEvent
{
    public bool Cancelled { get; set; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

/// <summary>
/// Ported from CCS MCREventListener.java
/// </summary>
public interface IMcEventListener
{
    void OnEvent(McEvent @event);
}

/// <summary>
/// Ported from CCS EventMap.java
/// Maps event types to listeners
/// </summary>
public class McEventMap
{
    private readonly Dictionary<Type, List<IMcEventListener>> _listeners = [];

    public void Register(Type eventType, IMcEventListener listener)
    {
        if (!_listeners.ContainsKey(eventType))
            _listeners[eventType] = [];
        _listeners[eventType].Add(listener);
    }

    public void Unregister(Type eventType, IMcEventListener listener)
    {
        if (_listeners.TryGetValue(eventType, out var list))
            list.Remove(listener);
    }

    public void Dispatch(McEvent @event)
    {
        var eventType = @event.GetType();
        if (_listeners.TryGetValue(eventType, out var listeners))
        {
            foreach (var listener in listeners)
                listener.OnEvent(@event);
        }
    }
}

/// <summary>
/// Ported from CCS ModAPI.java
/// </summary>
public class McModApi
{
    public string Id { get; set; } = "";
    public string Version { get; set; } = "1.0.0";
}

/// <summary>
/// Ported from CCS ModAPIManager.java
/// </summary>
public class McModApiManager
{
    private readonly List<McModApi> _apis = [];

    public void Register(McModApi api)
    {
        _apis.RemoveAll(a => a.Id == api.Id);
        _apis.Add(api);
    }

    public McModApi? Get(string id)
    {
        return _apis.Find(a => a.Id == id);
    }

    public bool IsAvailable(string id)
    {
        return _apis.Exists(a => a.Id == id);
    }
}