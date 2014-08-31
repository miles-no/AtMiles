using System.Linq;
using Contact.Domain;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Contact.ReadStore.SearchStore
{
    public class EmployeeSearchModelIndex : AbstractMultiMapIndexCreationTask<EmployeeSearchModelIndex.Result>
    {
        public class Result
        {
            public object Content { get; set; }
        }

        public EmployeeSearchModelIndex()
        {

            AddMap<EmployeeSearchModel>(personSearchModels =>
                from person in personSearchModels.Where(w=>w.Id != Constants.SystemUserId)
                select new
                {
                    Content = new[] { person.Name, person.Name, person.OfficeId, person.JobTitle, person.Email, 
                        string.Join<object>(" ", person.Competency.Select(s => s.InternationalCompentency)).Replace("#","sharp"), 
                        string.Join<object>(" ", person.Competency.Select(s => s.Competency)).Replace("#","sharp"),
                        string.Join<object>(" ", person.KeyQualifications)
                    }
                });

            Index(p => p.Content, FieldIndexing.Analyzed);
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