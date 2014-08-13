using System;
using System.Collections.Generic;

namespace Contact.Domain.CommandHandlers
{
    public class MainCommandHandler
    {
        private readonly Dictionary<Type, List<Action<Message>>> _routes = new Dictionary<Type, List<Action<Message>>>();

        internal void RegisterHandler<T>(Action<T> handler) where T : Message
        {
            List<Action<Message>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<Message, T>(x => handler(x)));
        }

        public void HandleCommand(Command cmd, Type type)
        {
            Handle(cmd, type);
        }

        private void Handle<T>(T command, Type originalType) where T : Command
        {
            List<Action<Message>> handlers;
            if (_routes.TryGetValue(originalType, out handlers))
            {
                if (handlers.Count != 1)
                {
                    throw new InvalidOperationException("cannot send to more than one handler");
                }
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("no handler registered");
            }
        }
    }
}
