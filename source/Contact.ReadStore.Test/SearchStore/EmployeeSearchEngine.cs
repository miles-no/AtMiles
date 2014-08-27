using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Indexes;

namespace Contact.ReadStore.Test.SearchStore
{
    public class EmployeeSearchEngine
    {
        private readonly IDocumentStore store;

        public EmployeeSearchEngine(IDocumentStore documentStore)
        {
            store = documentStore;
            IndexCreation.CreateIndexes(typeof(EmployeeSearchModelIndex).Assembly, store);
        }

   
        public List<EmployeeSearchModel> FulltextSearch(string searchString, int take, int skip, out int total)
        {
            searchString = searchString ?? string.Empty;
            //Maybe more special character handling is needed here
            searchString = searchString.Replace("#", "sharp");
            
            RavenQueryStatistics stats;
            var search = searchString
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => string.Format("{0}* ", x)).ToList();
            List<EmployeeSearchModel> results;
            using (var session = store.OpenSession())
            {
                
                var tmp = session.Query<EmployeeSearchModelIndex.Result, EmployeeSearchModelIndex>()
                    .Statistics(out stats);

                foreach (var s in search)
                {
                    tmp = tmp.Search(x => x.Content, s, 1, SearchOptions.And, EscapeQueryOptions.AllowPostfixWildcard);
                }

         
                results = tmp
                        .Skip(skip) 
                        .Take(take)
                        .As<EmployeeSearchModel>().ToList();
                
                foreach (var personSearchModel in results)
                {
                    personSearchModel.Score = session.Advanced.GetMetadataFor(personSearchModel).Value<double>("Temp-Index-Score");
                }
                
                total = stats.TotalResults;
            }

            return results;
        }

     
    }
}