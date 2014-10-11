using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace no.miles.at.Backend.ReadStore
{
    public class RavenDocumentStore
    {
        public static IDocumentStore CreateStore(string url)
        {
            var documentStore = new DocumentStore(){Url = url, DefaultDatabase = "Contact"};
             documentStore.Initialize();
            //documentStore.Conventions.RegisterIdConvention<UserLookupModel>((dbname, commands, user) => "users_lookup/" + user.Id);
            //documentStore.Conventions.RegisterIdConvention<EmployeeSearchModel>((dbname, commands, user) => "employee_search/" + user.Id);
            
            //Create all indexes in assembly
            IndexCreation.CreateIndexes(typeof(RavenDocumentStore).Assembly, documentStore);
            
            return documentStore;
        }
    }
}
