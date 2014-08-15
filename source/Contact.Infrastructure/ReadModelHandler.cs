using System;
using System.Collections.Generic;
using Contact.Domain;

namespace Contact.Infrastructure
{
    public class ReadModelHandler : IEventPublisher
    {
        private readonly Dictionary<Type, List<Action<Message>>> _routes = new Dictionary<Type, List<Action<Message>>>();

        public void RegisterHandler<T>(Action<T> handler) where T : Message
        {
            List<Action<Message>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<Message, T>(x => handler(x)));
        }

        public void Publish<T>(T @event) where T : Event
        {
            List<Action<Message>> handlers;
            if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;
            foreach (var handler in handlers)
            {
                handler(@event);
            }
        }
    }
}
