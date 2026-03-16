using System;
using System.Collections.Generic;

namespace TravesiaColombia.Core
{
    /// <summary>
    /// Sistema central de comunicación entre managers.
    /// Ningún manager llama directamente a otro — todo pasa por aquí.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers
            = new Dictionary<Type, List<Delegate>>();

        /// <summary>
        /// Suscribe un callback a un tipo de evento.
        /// Llama en OnEnable o Start del MonoBehaviour.
        /// </summary>
        public static void Subscribe<T>(Action<T> callback)
        {
            var eventType = typeof(T);
            if (!_subscribers.ContainsKey(eventType))
                _subscribers[eventType] = new List<Delegate>();
            _subscribers[eventType].Add(callback);
        }

        /// <summary>
        /// Elimina la suscripción. Llama en OnDisable o OnDestroy.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> callback)
        {
            var eventType = typeof(T);
            if (!_subscribers.TryGetValue(eventType, out var list)) return;
            list.Remove(callback);
            if (list.Count == 0) _subscribers.Remove(eventType);
        }

        /// <summary>
        /// Publica un evento a todos los suscriptores de ese tipo.
        /// </summary>
        public static void Publish<T>(T eventData)
        {
            var eventType = typeof(T);
            if (!_subscribers.TryGetValue(eventType, out var list)) return;
            foreach (var subscriber in list.ToArray())
                (subscriber as Action<T>)?.Invoke(eventData);
        }

        /// <summary>
        /// Limpia todas las suscripciones. Llama al cambiar de escena.
        /// </summary>
        public static void Clear() => _subscribers.Clear();
    }
}