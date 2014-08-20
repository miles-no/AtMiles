using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace Contact.Backend.MockStore
{

    public class MockStore 
    {
        public static IDocumentStore DocumentStore;

        static MockStore()
        {
            DocumentStore = new DocumentStore(){Url = "http://milescontact.cloudapp.net:8080", DefaultDatabase = "Contact"};
            DocumentStore.Initialize();
        }
        

    }
}
