using System.Collections.Generic;
using Contact.Domain.Events;

namespace Contact.Domain.Aggregates
{
    public class Company : AggregateRoot
    {
        private readonly List<string> _companyAdmins;

        public Company()
        {
            _companyAdmins = new List<string>();
        }

        public bool IsCompanyAdmin(string identifier)
        {
            return _companyAdmins.Contains(identifier);
        }

        public void AddCompanyAdmin(Employee employeeToBeAdmin)
        {
            var ev = new CompanyAdminAdded(employeeToBeAdmin.Id, employeeToBeAdmin.Name);
            ApplyChange(ev);
        }

        public void RemoveCompanyAdmin(Employee employeeToBeRemoved)
        {
            var ev = new CompanyAdminRemoved(employeeToBeRemoved.Id, employeeToBeRemoved.Name);
            ApplyChange(ev);
        }

        private void Apply(CompanyAdminAdded ev)
        {
            if (_companyAdmins.Contains(ev.NewAdminId)) return;

            _companyAdmins.Add(ev.NewAdminId);
        }

        private void Apply(CompanyAdminRemoved ev)
        {
            _companyAdmins.RemoveAll(item => item == ev.AdminId);
        }
    }
}
