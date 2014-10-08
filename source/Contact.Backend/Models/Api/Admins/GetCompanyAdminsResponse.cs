using System.Collections.Generic;

namespace Contact.Backend.Models.Api.Admins
{
    public class GetCompanyAdminsResponse
    {
        public List<Admin> Admins { get; set; }

        public class Admin
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}