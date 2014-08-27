using System.Configuration;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace Contact.Backend.MockStore
{

    public class RavenDocumentStore 
    {
        public static IDocumentStore CreateStore(string url)
        {
            var documentStore = new DocumentStore(){Url = url, DefaultDatabase = "Contact"};
             documentStore.Initialize();
            return documentStore;
        }

    }
}
