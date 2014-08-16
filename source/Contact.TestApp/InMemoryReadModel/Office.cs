using System.Collections.Generic;
using Contact.Domain.ValueTypes;

namespace Contact.TestApp.InMemoryReadModel
{
    public class Office
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public List<SimpleEmployee> Admins { get; private set; }
        public List<SimpleEmployee> Employees { get; private set; }

        public Office()
        {
            Admins = new List<SimpleEmployee>();
            Employees = new List<SimpleEmployee>();
        }
    }
}
