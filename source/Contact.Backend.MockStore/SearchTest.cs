using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client;

namespace Contact.Backend.MockStore
{
    public class SearchTest
    {
        
        public class SearchMaterial
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string[] Tags { get; set; }
        }

        private const string SearchIndexName = "SearchMaterialByName";

        public SearchTest()
        {
            Store = MockStore.DocumentStore;
        }

        public IDocumentStore Store { get; set; }

        public List<SearchMaterial> Search(string searchString)
        {
            List<SearchMaterial> results;
            using (var session = Store.OpenSession())
            {
                results = session.Query<SearchMaterial>(SearchIndexName)
                    .Search(x => x.FirstName, searchString, 10)
                    .Search(x => x.LastName, searchString, 10)
                    .Search(x => x.Tags, searchString)
                    .ToList();

            }
            return results;
        }

        public void Setup()
        {
            
            
            Store.DatabaseCommands.PutIndex(SearchIndexName, new IndexDefinition
            {
                Map = "from user in docs.SearchMaterials select new { user.FirstName, user.LastName, user.Tags }",
                Indexes = { { "FirstName", FieldIndexing.Analyzed },{"LastName", FieldIndexing.Analyzed},{"Tags", FieldIndexing.Analyzed} }
            }, true);

            var test1 = new SearchMaterial {Id = "SearchMaterials/1", FirstName = "Banana", LastName = "Split", Tags = new []{"Dessert","Old school"}};
            var test2 = new SearchMaterial {Id = "SearchMaterials/2", FirstName = "Jack", LastName = "Black", Tags = new[] { "Actor", "Rock school","Banana fan" } };

            using (var session = Store.OpenSession())
            {
                session.Store(test1);
                session.Store(test2);
                session.SaveChanges();
                // just be sure its indexed
                var results =
                    session.Query<SearchMaterial>(SearchIndexName)
                        .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(2)))
                        .ToList();
            }

        }
    }
}