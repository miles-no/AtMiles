using System.Threading.Tasks;
using Contact.Domain.Events.CommandSession;
using Contact.Infrastructure;
using Raven.Client;

namespace Contact.ReadStore.SessionStore
{
    public class CommandStatusStore
    {
        private readonly IDocumentStore _documentStore;

        public CommandStatusStore(IDocumentStore documentStore)
        {
            this._documentStore = documentStore;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            handler.RegisterHandler<CommandRequested>(HandleRequested);
            handler.RegisterHandler<CommandException>(HandleException);
            handler.RegisterHandler<CommandSucceded>(HandleSuccess);
        }

        private async Task HandleSuccess(CommandSucceded commandSucceded)
        {
            var commandSession = new CommandStatus {Id = CommandStatusConstants.Prefix + commandSucceded.CorrelationId, Status = CommandStatusConstants.OkStatus};
            
            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(commandSession);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleException(CommandException commandException)
        {
            var commandSession = new CommandStatus { Id = CommandStatusConstants.Prefix + commandException.CorrelationId, ErrorMessage = commandException.ExceptionMessage, Status = CommandStatusConstants.FailedStatus };

            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(commandSession);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleRequested(CommandRequested commandRequested)
        {
            var commandSession = new CommandStatus { Id = CommandStatusConstants.Prefix + commandRequested.CorrelationId, Status = CommandStatusConstants.PendingStatus };

            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(commandSession);
                await session.SaveChangesAsync();
            }
        }
    }
}