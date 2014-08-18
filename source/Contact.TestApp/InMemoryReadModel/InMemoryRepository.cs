using System.Collections.Generic;

namespace Contact.TestApp.InMemoryReadModel
{
    public class InMemoryRepository
    {
        public List<Company> Companies { get; set; }
        public List<Employee> Employees { get; set; }

        public InMemoryRepository()
        {
            Companies = new List<Company>();
            Employees = new List<Employee>();
        }
    }
}
