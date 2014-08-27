using Raven.Client;

namespace Contact.ReadStore.SessionStore
{
    public class CommandStatusEngine
    {
        private readonly IDocumentStore documentStore;

        public CommandStatusEngine(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public CommandStatus GetStatus(string commandId)
        {
            CommandStatus res;
            using (var session = documentStore.OpenSession())
            {
                var id = CommandStatusConstants.Prefix + commandId;
                res = session.Load<CommandStatus>(id);
            }
            if (res == null)
            {
                res = new CommandStatus{Id = commandId, Status = CommandStatusConstants.NotRegisteredStatus};
            }

            return res;
        }
    }
}