using System.Collections.Generic;

namespace Contact.ReadStore.UserStore
{
    public class UserLookupModel
    {
        public string Id { get; set; }
        public string GlobalId { get; set; }
        public string Email { get; set; }
        public string GlobalProviderId { get; set; }
        public string GlobalProviderEmail { get; set; }
        public string Name { get; set; }

        public bool CompanyAdmin { get; set; }
        public string CompanyId { get; set; }
    }
}
