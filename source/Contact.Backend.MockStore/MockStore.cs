using Raven.Client;
using Raven.Client.Embedded;

namespace Contact.Backend.MockStore
{

    public class MockStore 
    {
        public static IDocumentStore DocumentStore;

        static MockStore()
        {
            DocumentStore = new EmbeddableDocumentStore();
            DocumentStore.Initialize();
        }
        

    }
}
