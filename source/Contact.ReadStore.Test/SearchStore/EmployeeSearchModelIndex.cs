using System.Linq;
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