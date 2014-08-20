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
                res = session.Query<CommandStatus>().FirstOrDefault(w => w.Id == CommandStatusConstants.Prefix + commandId);
            }
            if (res == null)
            {
                res = new CommandStatus{Id = commandId, Status = CommandStatusConstants.NotRegisteredStatus};
            }

            return res;
        }
    }
}