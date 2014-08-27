using System.Collections.Generic;

namespace Contact.ReadStore.UserStore
{
    public class User
    {
        private List<string> adminForOffices;
        public string Id { get; set; }
        public string Email { get; set; }
        public string GlobalId { get; set; }
        public string LoginId { get; set; }
        public string Name { get; set; }

        public List<string> AdminForOffices
        {
            get
            {
                if (adminForOffices == null)
                {
                    adminForOffices = new List<string>();
                }
                return adminForOffices;
            }
            set { adminForOffices = value; }
        }

        public bool CompanyAdmin { get; set; }
        public string CompanyId { get; set; }
        public bool ValidUser { get { return string.IsNullOrEmpty(GlobalId) == false; } }
    }
}
