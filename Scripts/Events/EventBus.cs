using System;
using System.Collections.Generic;

namespace PlayerSpace;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> listeners = new Dictionary<Type, List<Delegate>>();

    public static void Subscribe<T>(Action<T> handler)
    {
        List<Delegate> list;
        if (!listeners.TryGetValue(typeof(T), out list))
        {
            list = new List<Delegate>();
            listeners[typeof(T)] = list;
        }
        list.Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        List<Delegate> list;
        if (listeners.TryGetValue(typeof(T), out list))
        {
            list.Remove(handler);
        }
    }

    public static void Publish<T>(T gameEvent)
    {
        List<Delegate> list;
        if (listeners.TryGetValue(typeof(T), out list))
        {
            Delegate[] copy = list.ToArray();
            foreach (Delegate d in copy)
            {
                Action<T> action = (Action<T>)d;
                if (action != null)
                {
                    action(gameEvent);
                }
            }
        }
    }
}
