using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client;

namespace Contact.Backend.MockStore
{
    public class SearchTest
    {
        
      
//TODO: hekte på fra testapp-
        //public class PersonSearchModel
        //{
        //    public string Id { get; set; }
        //    public string CompanyId { get; set; }
        //    public string OfficeId { get; set; }
        //    public string GlobalId { get; set; }
        //    public string Name { get; set; }
        //    public DateTime DateOfBirth { get; set; }
        //    public string JobTitle { get; set; }
        //    public string PhoneNumber { get; set; }
        //    public string Email { get; set; }
        //    public string Thumb { get; set; }
        //}

        private const string SearchIndexName = "SearchPerson";

        public SearchTest()
        {
            Store = MockStore.DocumentStore;
        }

        public IDocumentStore Store { get; set; }

        //public List<PersonSearchModel> Search(string searchString)
        //{
        //    List<PersonSearchModel> results;
        //    using (var session = Store.OpenSession())
        //    {
        //        results = session.Query<PersonSearchModel>(SearchIndexName)
        //            .Search(x => x.Name, searchString, 10)
        //            .Search(x => x.JobTitle, searchString)
        //            .Search(x => x.OfficeId, searchString)
        //            .ToList();

        //    }
        //    return results;
        //}

        public void Setup()
        {
            
            
            //Store.DatabaseCommands.PutIndex(SearchIndexName, new IndexDefinition
            //{
            //    Map = "from user in docs.SearchMaterials select new { user.FirstName, user.LastName, user.Tags }",
            //    Indexes = { { "FirstName", FieldIndexing.Analyzed },{"LastName", FieldIndexing.Analyzed},{"Tags", FieldIndexing.Analyzed} }
            //}, true);

            //var test1 = new PersonSearchModel {Id = "SearchMaterials/1", Name = "Banana Split", LastName = "Split", Tags = new []{"Dessert","Old school"}};
            //var test2 = new PersonSearchModel {Id = "SearchMaterials/2", Name = "Jack", LastName = "Black", Tags = new[] { "Actor", "Rock school","Banana fan" } };

            //using (var session = Store.OpenSession())
            //{
            //    session.Store(test1);
            //    session.Store(test2);
            //    session.SaveChanges();
            //    // just be sure its indexed
            //    var results =
            //        session.Query<PersonSearchModel>(SearchIndexName)
            //            .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(2)))
            //            .ToList();
            //}

        }
    }
}