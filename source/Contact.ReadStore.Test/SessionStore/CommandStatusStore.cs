using Contact.Backend.MockStore;
using Contact.Domain.Events.CommandSession;
using Contact.Domain.Events.Employee;
using Contact.Infrastructure;

namespace Contact.ReadStore.Test.SessionStore
{
    public class CommandStatusStore
    {
       
        public void PrepareHandler(ReadModelHandler handler)
        {
            handler.RegisterHandler<CommandRequested>(HandleRequested);
            handler.RegisterHandler<CommandException>(HandleException);
            handler.RegisterHandler<CommandSucceded>(HandleSuccess);
        }

        private void HandleSuccess(CommandSucceded commandSucceded)
        {
            var commandSession = new CommandStatus {Id = CommandStatusConstants.Prefix + commandSucceded.CorrelationId, Status = CommandStatusConstants.OkStatus};
            
            using (var session = MockStore.DocumentStore.OpenSession())
            {
                session.Store(commandSession);
                session.SaveChanges();
            }
        }

        private void HandleException(CommandException commandException)
        {
            var commandSession = new CommandStatus { Id = CommandStatusConstants.Prefix + commandException.CorrelationId, ErrorMessage = commandException.ExceptionMessage, Status = CommandStatusConstants.FailedStatus };

            using (var session = MockStore.DocumentStore.OpenSession())
            {
                session.Store(commandSession);
                session.SaveChanges();
            }
        }

        private void HandleRequested(CommandRequested commandRequested)
        {
            var commandSession = new CommandStatus { Id = CommandStatusConstants.Prefix + commandRequested.CorrelationId, Status = CommandStatusConstants.PendingStatus };

            using (var session = MockStore.DocumentStore.OpenSession())
            {
                session.Store(commandSession);
                session.SaveChanges();
            }
        }
    }
}