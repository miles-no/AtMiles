using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contact.Domain.CommandHandlers
{
    public class MainCommandHandler
    {
        private readonly Dictionary<Type, List<Func<Message, Task>>> _routes = new Dictionary<Type, List<Func<Message, Task>>>();

        internal void RegisterHandler<T>(Func<T,Task> handler) where T : Message
        {
            List<Func<Message, Task>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Func<Message, Task>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<Message, T>(x => handler(x)));
        }

        public async Task HandleCommand(Command cmd, Type type)
        {
            await Handle(cmd, type);
        }

        private async Task Handle<T>(T command, Type originalType) where T : Command
        {
            List<Func<Message, Task>> handlers;
            if (_routes.TryGetValue(originalType, out handlers))
            {
                if (handlers.Count != 1)
                {
                    throw new InvalidOperationException("cannot send to more than one handler");
                }
                await handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("no handler registered");
            }
        }
    }
}
