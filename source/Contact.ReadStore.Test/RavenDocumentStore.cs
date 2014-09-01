using Contact.ReadStore.SearchStore;
using Contact.ReadStore.UserStore;
using Raven.Client;
using Raven.Client.Document;

namespace Contact.ReadStore
{
    public class RavenDocumentStore
    {
        public static IDocumentStore CreateStore(string url)
        {
            var documentStore = new DocumentStore(){Url = url, DefaultDatabase = "Contact"};
             documentStore.Initialize();
            //documentStore.Conventions.RegisterIdConvention<UserLookupModel>((dbname, commands, user) => "users_lookup/" + user.Id);
            //documentStore.Conventions.RegisterIdConvention<EmployeeSearchModel>((dbname, commands, user) => "employee_search/" + user.Id);

            using (var session = documentStore.OpenSession())
            {
                Raven.Client.Indexes.IndexCreation.CreateIndexes(
                    typeof(UserLookupIndex).Assembly, documentStore);
                Raven.Client.Indexes.IndexCreation.CreateIndexes(
                    typeof(EmployeeSearchModelIndex).Assembly, documentStore);
                Raven.Client.Indexes.IndexCreation.CreateIndexes(
                    typeof(EmployeeSearchModelLookupIndex).Assembly, documentStore);
            }
            return documentStore;
        }

    }
}
