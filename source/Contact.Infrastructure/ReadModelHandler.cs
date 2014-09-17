﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contact.Domain;

namespace Contact.Infrastructure
{
    public class ReadModelHandler : IEventPublisher
    {
        private readonly Dictionary<Type, List<Func<Message, Task>>> _routes = new Dictionary<Type, List<Func<Message, Task>>>();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : Message
        {
            List<Func<Message, Task>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Func<Message, Task>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<Message, T>(x => handler(x)));
        }

        public void Publish<T>(T @event) where T : Event
        {
            List<Func<Message, Task>> handlers;
            if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;
            foreach (var handler in handlers)
            {
                handler(@event);
            }
        }
    }
}
