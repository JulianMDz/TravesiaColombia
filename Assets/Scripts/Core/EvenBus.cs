using UnityEngine;

public static class EventBus
{
    static readonly Dictionary<Type, List<Delegate>> _listeners = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        var t = typeof(T);
        if (!_listeners.ContainsKey(t)) _listeners[t] = new List<Delegate>();
        _listeners[t].Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        var t = typeof(T);
        if (_listeners.ContainsKey(t)) _listeners[t].Remove(callback);
    }

    public static void Publish<T>(T eventData)
    {
        var t = typeof(T);
        if (!_listeners.ContainsKey(t)) return;
        foreach (var listener in _listeners[t].ToArray())
            (listener as Action<T>)?.Invoke(eventData);
    }

    public static void Clear() => _listeners.Clear();
}
}
