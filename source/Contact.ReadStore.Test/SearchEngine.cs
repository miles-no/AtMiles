using System;
using System.Collections.Generic;
using System.Linq;
using Contact.Backend.MockStore;
using Contact.Domain.ValueTypes;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq.Indexing;

namespace Contact.ReadStore.Test
{
    public class SearchEngine
    {
        private readonly IDocumentStore store = MockStore.DocumentStore;

        public SearchEngine()
        {
            IndexCreation.CreateIndexes(typeof(PersonSearchModelIndex).Assembly, store);
        }


        public List<PersonSearchModel> FulltextSearch(string searchString, int take, int skip, out int total)
        {
            searchString = searchString ?? string.Empty;
            //Maybe more special character handling is needed here
            searchString = searchString.Replace("#", "sharp");
            
            RavenQueryStatistics stats;
            var search = searchString
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => string.Format("{0}* ", x)).ToList();
            List<PersonSearchModel> results;
            using (var session = store.OpenSession())
            {
                
                var tmp = session.Query<PersonSearchModelIndex.Result, PersonSearchModelIndex>()
                    .Statistics(out stats);

                foreach (var s in search)
                {
                    tmp = tmp.Search(x => x.Content, s, 1, SearchOptions.And, EscapeQueryOptions.AllowPostfixWildcard);
                }

         
                results = tmp
                        .Skip(skip) 
                        .Take(take)
                        .As<PersonSearchModel>().ToList();
                
                foreach (var personSearchModel in results)
                {
                    personSearchModel.Score = session.Advanced.GetMetadataFor(personSearchModel).Value<double>("Temp-Index-Score");
                }
                
                total = stats.TotalResults;
            }

            return results;
        }

     
    }

    public class PersonSearchModelIndex : AbstractMultiMapIndexCreationTask<PersonSearchModelIndex.Result>
    {
        public class Result
        {
            public object Content { get; set; }
        }

        public PersonSearchModelIndex()
        {
          
            AddMap<PersonSearchModel>(personSearchModels =>
                from person in personSearchModels
                select new
                {
                    Content = new[] { person.Name, person.Name, person.OfficeId, person.JobTitle, person.Email, 
                        string.Join(" ", person.Competency.Select(s => s.InternationalCompentency)).Replace("#","sharp"), 
                        string.Join(" ", person.Competency.Select(s => s.Competency)).Replace("#","sharp") }
                });

            Index(p => p.Content, FieldIndexing.Analyzed);
        }
    }
}