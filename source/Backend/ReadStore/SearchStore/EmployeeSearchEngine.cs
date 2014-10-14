using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;

namespace no.miles.at.Backend.ReadStore.SearchStore
{
    public class EmployeeSearchEngine
    {
        private readonly IDocumentStore _documentStore;

        public EmployeeSearchEngine(IDocumentStore documentDocumentStore)
        {
            _documentStore = documentDocumentStore;
        }

        public EmployeeSearchModel GetEmployeeSearchModel(string employeeId)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<EmployeeSearchModel>(EmployeeSearchStore.GetRavenId(employeeId));
            }
        }

   
        public IEnumerable<EmployeeSearchModel> FulltextSearch(string searchString, int take, int skip, out int total)
        {
            searchString = searchString ?? string.Empty;
            //Maybe more special character handling is needed here
            searchString = searchString.Replace("#", "sharp");

            var search = searchString
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => string.Format("{0}* ", x)).ToList();
            List<EmployeeSearchModel> results;
            using (var session = _documentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                var tmp = session.Query<EmployeeSearchModelIndex.Result, EmployeeSearchModelIndex>()
                    .Statistics(out stats);

                tmp = search.Aggregate(tmp, (current, s) => current.Search(x => x.Content, s, 1, SearchOptions.And, EscapeQueryOptions.AllowPostfixWildcard));


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