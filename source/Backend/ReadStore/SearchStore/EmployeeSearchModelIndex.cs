using System.Linq;
using no.miles.at.Backend.Domain;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace no.miles.at.Backend.ReadStore.SearchStore
{
    public class EmployeeSearchModelIndex : AbstractMultiMapIndexCreationTask<EmployeeSearchModelIndex.Result>
    {
        public class Result
        {
            public object Content { get; set; }
            public string Company { get; set; }
        }

        public EmployeeSearchModelIndex()
        {

            AddMap<EmployeeSearchModel>(personSearchModels =>
                from person in personSearchModels.Where(w=>w.Id != Constants.SystemUserId)
                select new
                {
                    Company = person.CompanyId,
                    Content = new[] { person.Name, person.Name, person.OfficeName, person.JobTitle, person.Email, 
                        string.Join<object>(" ", person.Competency.Select(s => s.InternationalCompentency)).Replace("#","sharp"), 
                        string.Join<object>(" ", person.Competency.Select(s => s.Competency)).Replace("#","sharp"),
                        string.Join<object>(" ", person.KeyQualifications)
                    }
                });

            Index(p => p.Content, FieldIndexing.Analyzed);
            Index(p => p.Company, FieldIndexing.NotAnalyzed);
        }
    }

    public class EmployeeSearchModelLookupIndex : AbstractIndexCreationTask<EmployeeSearchModel>
    {
        public EmployeeSearchModelLookupIndex()
        {
            Map = u =>
                from person in u
                select new
                {
                    person.Id,
                    person.CompanyId,
                    person.Email,
                };

            Index(p => p.CompanyId, FieldIndexing.NotAnalyzed);
            Index(p => p.Id, FieldIndexing.NotAnalyzed);
            Index(p => p.Email, FieldIndexing.NotAnalyzed);
    
        }
    }

}