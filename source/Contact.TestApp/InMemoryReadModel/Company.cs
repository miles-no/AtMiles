using System.Collections.Generic;
using System.Linq;

namespace Contact.TestApp.InMemoryReadModel
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<SimpleEmployee> Admins { get; private set; }
        public List<Office> Offices { get; private set; }

        public Company()
        {
            Admins = new List<SimpleEmployee>();
            Offices = new List<Office>();
        }

        public void AddOffice(Office office)
        {
            Offices.Add(office);
        }

        public void AddCompanyAdmin(SimpleEmployee newAdmin)
        {
            Admins.Add(newAdmin);
        }

        public void RemoveCompanyAdmin(SimpleEmployee admin)
        {
            Admins.RemoveAll(a => a.Id == admin.Id);
        }

        public void AddEmployeeToOffice(SimpleEmployee employee, string officeId)
        {
            var office = GetOffice(officeId);
            office.Employees.Add(employee);
        }

        private Office GetOffice(string officeId)
        {
            return Offices.FirstOrDefault(c => c.Id == officeId);
        }

        public void RemoveOffice(string officeId)
        {
            Offices.RemoveAll(o => o.Id == officeId);
        }

        public void RemoveOfficeAdmin(string officeId, SimpleEmployee admin)
        {
            var office = GetOffice(officeId);
            office.Admins.RemoveAll(a => a.Id == admin.Id);
        }

        public void AddOfficeAdmin(string officeId, SimpleEmployee admin)
        {
            var office = GetOffice(officeId);
            office.Admins.Add(admin);
        }

        public void RemoveEmployeeFromOffice(string employeeId, string officeId)
        {
            var office = GetOffice(officeId);
            office.Employees.RemoveAll(e => e.Id == employeeId);
        }
    }
}
