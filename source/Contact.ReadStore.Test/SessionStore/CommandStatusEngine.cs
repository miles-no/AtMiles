using System.Linq;
using Contact.Backend.MockStore;
using Raven.Client.Linq;

namespace Contact.ReadStore.Test.SessionStore
{
    public class CommandStatusEngine
    {
        public CommandStatus GetStatus(string commandId)
        {
            CommandStatus res;
            using (var session = MockStore.DocumentStore.OpenSession())
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