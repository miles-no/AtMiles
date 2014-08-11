using System.Diagnostics;
using Contact.Domain;
using Newtonsoft.Json;

namespace Contact.Backend.Infrastructure
{
    public class CommandSenderMock : ICommandSender
    {
        public void Send<T>(T command) where T : Command
        {
            Debug.WriteLine("#####################################");    
            Debug.WriteLine("Command:\n");    
            Debug.WriteLine(JsonConvert.SerializeObject(command));
            Debug.WriteLine("\n#####################################");    
        }
    }
}