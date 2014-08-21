using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Contact.ReadStore.Test.UserStore
{
    
    public class UserLookupIndex : AbstractIndexCreationTask<User>
    {
        public UserLookupIndex()
        {

            Map = u =>
                from person in u
                select new
                {
                    person.GlobalId,
                    person.CompanyId
                };

            Index(p => p.CompanyId, FieldIndexing.NotAnalyzed);
            Index(p => p.GlobalId, FieldIndexing.NotAnalyzed);
        }
    }
}