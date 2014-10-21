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
            _documentStore = documentStore;
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

        private async Task HandleSuccess(CommandSucceded ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var model = await session.LoadAsync<CommandStatus>(GetRavenId(ev.CorrelationId));
                model = Patch(model, ev);
                await session.StoreAsync(model);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleException(CommandException ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var model = await session.LoadAsync<CommandStatus>(GetRavenId(ev.CorrelationId));
                model = Patch(model, ev);
                await session.StoreAsync(model);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleRequested(CommandRequested ev)
        {
            var commandSession = new CommandStatus
            {
                Id = GetRavenId(ev.CorrelationId),
                CommandName = ev.CommandName,
                Status = CommandStatusConstants.PendingStatus,
                Started = ev.Created
            };

            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(commandSession);
                await session.SaveChangesAsync();
            }
        }

        private CommandStatus Patch(CommandStatus model, CommandSucceded ev)
        {
            model.Finished = ev.Created;
            model.Status = CommandStatusConstants.OkStatus;
            return model;
        }

        private CommandStatus Patch(CommandStatus model, CommandException ev)
        {
            model.Finished = ev.Created;
            model.ErrorMessage = ev.ExceptionMessage;
            model.Status = CommandStatusConstants.FailedStatus;
            return model;
        }
    }
}