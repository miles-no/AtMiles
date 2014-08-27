using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Contact.ReadStore.UserStore
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
                    person.LoginId,
                    person.CompanyId,
                    person.Email,
                    person.AdminForOffices,
                    person.CompanyAdmin
                };

            Index(p => p.CompanyId, FieldIndexing.NotAnalyzed);
            Index(p => p.GlobalId, FieldIndexing.NotAnalyzed);
            Index(p => p.Email, FieldIndexing.NotAnalyzed);
            Index(p => p.AdminForOffices, FieldIndexing.NotAnalyzed);
            Index(p => p.CompanyAdmin, FieldIndexing.NotAnalyzed);
            Index(p => p.LoginId, FieldIndexing.NotAnalyzed);
        }
    }
}