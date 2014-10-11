using System.Diagnostics;
using Newtonsoft.Json;
using no.miles.at.Backend.Domain;

namespace no.miles.at.Backend.Infrastructure
{
    public class CommandSenderMock : ICommandSender
    {
        public void Send<T>(T command) where T : Command
        {
            if (command == null)
            {
                  return;
            }

            Debug.WriteLine("#####################################");
            Debug.WriteLine("Command: " + command.GetType().Name + "\n");
            Debug.WriteLine(JsonConvert.SerializeObject(command));
            Debug.WriteLine("\n#####################################");
        }
    }
}