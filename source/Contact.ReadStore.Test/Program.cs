using System.Configuration;
using Contact.Backend.MockStore;

namespace Contact.ReadStore
{
    class Program
    {
        private static void Main(string[] args)
        {
            var admin = new ReadStoreAdmin();
            admin.PrepareHandlers(RavenDocumentStore.CreateStore(ConfigurationManager.AppSettings["ravenUrl"]));
            admin.StartListening();
        }
       
    }
}
