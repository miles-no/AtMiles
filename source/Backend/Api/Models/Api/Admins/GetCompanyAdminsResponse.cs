using System.Collections.Generic;

namespace no.miles.at.Backend.Api.Models.Api.Admins
{
    public class GetCompanyAdminsResponse
    {
        public List<Admin> Admins { get; set; }

        public class Admin
        {
            public string Id;
            public string Name;
        }
    }
}