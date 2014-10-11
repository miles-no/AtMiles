using System.Threading.Tasks;
using no.miles.at.Backend.Domain.Events.CommandSession;
using no.miles.at.Backend.Infrastructure;
using Raven.Client;

namespace no.miles.at.Backend.ReadStore.SessionStore
{
    public class CommandStatusStore
    {
        private readonly IDocumentStore _documentStore;

        public CommandStatusStore(IDocumentStore documentStore)
        {
            this._documentStore = documentStore;
        }

        public static string GetRavenId(string id)
        {
            return "CommandStatus/" + id;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            handler.RegisterHandler<CommandRequested>(HandleRequested);
            handler.RegisterHandler<CommandException>(HandleException);
            handler.RegisterHandler<CommandSucceded>(HandleSuccess);
        }

        private async Task HandleSuccess(CommandSucceded commandSucceded)
        {
            var commandSession = new CommandStatus {Id = GetRavenId(commandSucceded.CorrelationId), Status = CommandStatusConstants.OkStatus};
            
            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(commandSession);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleException(CommandException commandException)
        {
            var commandSession = new CommandStatus { Id = GetRavenId(commandException.CorrelationId), ErrorMessage = commandException.ExceptionMessage, Status = CommandStatusConstants.FailedStatus };

            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(commandSession);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleRequested(CommandRequested commandRequested)
        {
            var commandSession = new CommandStatus { Id = GetRavenId(commandRequested.CorrelationId), Status = CommandStatusConstants.PendingStatus };

            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(commandSession);
                await session.SaveChangesAsync();
            }
        }
    }
}