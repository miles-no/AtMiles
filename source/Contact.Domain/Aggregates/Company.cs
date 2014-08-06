using System.Collections.Generic;
using Contact.Domain.Entities;
using Contact.Domain.Events;
using System.Linq;

namespace Contact.Domain.Aggregates
{
    public class Company : AggregateRoot
    {
        private readonly List<string> _companyAdmins;
        private readonly List<Office> _offices; 

        public Company()
        {
            _companyAdmins = new List<string>();
            _offices = new List<Office>();
        }

        public bool IsCompanyAdmin(string identifier)
        {
            return _companyAdmins.Contains(identifier);
        }

        public bool IsOfficeAdmin(string identifier, string officeId)
        {
            if (!IsOffice(officeId)) return false;
            var office = GetOffice(officeId);
            return office.IsAdmin(identifier);
        }

        private Office GetOffice(string officeId)
        {
            return _offices.First(item => item.Id == officeId);
        }

        private bool IsOffice(string officeId)
        {
            return _offices.Any(item => item.Id == officeId);
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

        private void Apply(OfficeOpened ev)
        {
            if (IsOffice(ev.Id)) return;

            var office = new Office(ev.Id);
            _offices.Add(office);
        }

        private void Apply(OfficeClosed ev)
        {
            if (!IsOffice(ev.OfficeId)) return;

            _offices.RemoveAll(item => item.Id == ev.OfficeId);
        }

        private void Apply(OfficeAdminAdded ev)
        {
            if (!IsOffice(ev.OfficeId)) return;

            var office = GetOffice(ev.OfficeId);
            office.AddAdmin(ev.AdminId);
        }

        private void Apply(OfficeAdminRemoved ev)
        {
            if (!IsOffice(ev.OfficeId)) return;
            var office = GetOffice(ev.OfficeId);
            office.RemoveAdmin(ev.AdminId);
        }
    }
}
