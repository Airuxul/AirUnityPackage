using System;
using System.Collections.Generic;
using CodiceApp.EventTracking.Plastic;

namespace UntiyGameCore.Runtime.Event
{
    public interface IEvent
    {
        public int ListenersCount { get;  }
    }

    public class EventImpl : IEvent
    {
        public Action Action;
        
        public int ListenersCount => Action?.GetInvocationList().Length ?? 0;
        
        public EventImpl(Action action)
        {
            Action = action;
        }
    }

    public class EventImpl<T> : IEvent
    {
        public Action<T> Action;
        
        public int ListenersCount => Action?.GetInvocationList().Length ?? 0;
        
        public EventImpl(Action<T> action)
        {
            Action = action;
        }
    }
    
    public class EventManager
    {
        private readonly Dictionary<string, IEvent> _events = new();
        
        public void RegisterEvent<T>(string eventName, Action<T> callback)
        {
            if (_events.TryGetValue(eventName, out var @event))
            {
                if (@event is EventImpl<T> existingEvent)
                    existingEvent.Action += callback;
            }
            else
            {
                _events[eventName] = new EventImpl<T>(callback);
            }
        }

        public void RegisterEvent(string eventName, Action callback)
        {
            if (_events.TryGetValue(eventName, out var @event))
            {
                if (@event is EventImpl existingEvent)
                    existingEvent.Action += callback;
            }
            else
            {
                _events[eventName] = new EventImpl(callback);
            }
        }
        
        public void TriggerEvent<T>(string eventName, T parma)
        {
            if (_events.TryGetValue(eventName, out var @event) && @event is EventImpl<T> eventImpl)
            {
                eventImpl.Action.Invoke(parma);
            }
        }

        public void TriggerEvent(string eventName)
        {
            if (_events.TryGetValue(eventName, out var @event) && @event is EventImpl eventImpl)
            {
                eventImpl.Action.Invoke();
            }
        }
        
        public void UnregisterEvent<T>(string eventName, Action<T> callback)
        {
            if (!_events.TryGetValue(eventName, out var @event)) return;
            if (@event is not EventImpl<T> existingEvent) return;
            existingEvent.Action -= callback;
            if (existingEvent.ListenersCount == 0)
                RemoveEvent(eventName);
        }

        public void UnregisterEvent(string eventName, Action callback)
        {
            if (!_events.TryGetValue(eventName, out var @event)) return;
            if (@event is not EventImpl existingEvent) return;
            existingEvent.Action -= callback;
            if (existingEvent.ListenersCount == 0)
                RemoveEvent(eventName);
        }
        
        private void RemoveEvent(string eventName)
        {
            _events.Remove(eventName);
        }

        private void Clear()
        {
            _events.Clear();
        }
    }
}