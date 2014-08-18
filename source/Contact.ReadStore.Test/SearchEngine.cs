using System;
using System.Collections.Generic;
using System.Linq;
using Contact.Backend.MockStore;
using Raven.Abstractions.Indexing;
using Raven.Client;

namespace Contact.ReadStore.Test
{
    public class SearchEngine
    {
        readonly IDocumentStore store = MockStore.DocumentStore;
        private const string SearchIndexName = "SearchEmployee";

        public void SetupIndexes()
        {
            store.DatabaseCommands.PutIndex(SearchIndexName, new IndexDefinition
            {
                Map = "from user in docs.PersonSearchModels select new { user.Name, user.Query }",
                Indexes = { { "Name", FieldIndexing.Analyzed }, { "Query", FieldIndexing.Analyzed } }
            }, true);

        }
        
        public List<PersonSearchModel> FulltextSearch(string searchString, int take)
        {
            RavenQueryStatistics stats;
                
            var search = searchString
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => string.Format("{0}* ", x));

            List<PersonSearchModel> results;
            using (var session = store.OpenSession())
            {
                var tmp = session.Query<PersonSearchModel>(SearchIndexName)
                    .Statistics(out stats);

                foreach (var s in search)
                {
                    tmp = tmp.Search(x => x.Query, s, 1, SearchOptions.And, EscapeQueryOptions.AllowPostfixWildcard);
                }
               
               results = tmp.Take(take).ToList();
            }
        
            return results;
        }
    }
}