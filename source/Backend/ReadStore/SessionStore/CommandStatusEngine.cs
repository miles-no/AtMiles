using Raven.Client;

namespace no.miles.at.Backend.ReadStore.SessionStore
{
    public class CommandStatusEngine
    {
        private readonly IDocumentStore _documentStore;

        public CommandStatusEngine(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public CommandStatus GetStatus(string commandId)
        {
            CommandStatus res;
            using (var session = _documentStore.OpenSession())
            {
                var id = CommandStatusStore.GetRavenId(commandId);
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