using System;
using System.Collections.Generic;
using Contact.Domain.Entities;
using Contact.Domain.Events;
using System.Linq;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Aggregates
{
    public class Company : AggregateRoot
    {
        private readonly List<string> _companyAdmins;
        private readonly List<Office> _offices;
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public Company()
        {
            _name = string.Empty;
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

        public Office GetOffice(string officeId)
        {
            return _offices.First(item => item.Id == officeId);
        }

        public bool IsOffice(string officeId)
        {
            return _offices.Any(item => item.Id == officeId);
        }

        public void AddCompanyAdmin(Employee employeeToBeAdmin, Person createdBy, string correlationId)
        {
            if(IsCompanyAdmin(employeeToBeAdmin.Id)) throw new AlreadyExistingItemException();

            var ev = new CompanyAdminAdded(_id, _name, employeeToBeAdmin.Id, employeeToBeAdmin.Name)
                .WithCorrelationId(correlationId)
                .WithCreated(DateTime.UtcNow)
                .WithCreatedBy(createdBy);
            ApplyChange(ev);
        }

        public void RemoveCompanyAdmin(Employee employeeToBeRemoved, Person createdBy, string correlationId)
        {
            if(OnlyOneCompanyAdminLeft()) throw new LastItemException();
            if (employeeToBeRemoved.Id == createdBy.Identifier) throw new NoAccessException();

            var ev = new CompanyAdminRemoved(_id, _name, employeeToBeRemoved.Id, employeeToBeRemoved.Name)
                .WithCorrelationId(correlationId)
                .WithCreated(DateTime.UtcNow)
                .WithCreatedBy(createdBy);
            ApplyChange(ev);
        }

        private bool OnlyOneCompanyAdminLeft()
        {
            return _companyAdmins.Count == 1;
        }

        public void OpenOffice(string name, Address address, Person createdBy, string correlationId)
        {
            var officeId = Guid.NewGuid().ToString();
            var ev = new OfficeOpened(_id, _name, officeId, name, address)
                .WithCorrelationId(correlationId)
                .WithCreated(DateTime.UtcNow)
                .WithCreatedBy(createdBy);
            ApplyChange(ev);
        }

        public void CloseOffice(string officeId, Person createdBy, string correlationId)
        {
            if (OnlyOneOfficeLeft()) throw new LastItemException();

            var office = GetOffice(officeId);

            if(!office.IsEmptyForEmployees()) throw new ExistingChildItemsException();

            var ev = new OfficeClosed(_id, _name, office.Id, office.Name)
                .WithCorrelationId(correlationId)
                .WithCreated(DateTime.UtcNow)
                .WithCreatedBy(createdBy);
            ApplyChange(ev);
        }

        public void AddOfficeAdmin(string officeId, Employee newAdmin, Person createdBy, string correlationId)
        {
            if(!IsOffice(officeId)) throw new UnknownItemException();
            
            var office = GetOffice(officeId);

            if (!HasOfficeAccess(createdBy.Identifier, office.Id)) throw new NoAccessException();

            if(office.IsAdmin(newAdmin.Id)) throw new AlreadyExistingItemException();

            var ev = new OfficeAdminAdded(_id, _name, office.Id, office.Name,newAdmin.Id, newAdmin.Name)
                .WithCorrelationId(correlationId)
                .WithCreated(DateTime.UtcNow)
                .WithCreatedBy(createdBy);
            ApplyChange(ev);
        }

        public void RemoveOfficeAdmin(string officeId, Employee adminToBeRemoved, Person createdBy, string correlationId)
        {
            if (!IsOffice(officeId)) throw new UnknownItemException();

            var office = GetOffice(officeId);

            if (!HasOfficeAccess(createdBy.Identifier, office.Id)) throw new NoAccessException();
            
            CheckIfTryingToDisconnectSelf(adminToBeRemoved, createdBy);

            var ev = new OfficeAdminRemoved(_id, _name, office.Id, office.Name, adminToBeRemoved.Id, adminToBeRemoved.Name)
                .WithCorrelationId(correlationId)
                .WithCreated(DateTime.UtcNow)
                .WithCreatedBy(createdBy);
            ApplyChange(ev);
        }

        private void CheckIfTryingToDisconnectSelf(Employee adminToBeRemoved, Person createdBy)
        {
            if (adminToBeRemoved.Id == createdBy.Identifier)
            {
                if (!IsCompanyAdmin(adminToBeRemoved.Id)) throw new NoAccessException();
            }
        }

        private bool OnlyOneOfficeLeft()
        {
            return _offices.Count == 1;
        }


        private void Apply(CompanyCreated ev)
        {
            _id = ev.CompanyId;
            _name = ev.CompanyName;
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
            if (IsOffice(ev.OfficeId)) return;

            var office = new Office(ev.OfficeId, ev.OfficeName);
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

        private void Apply(EmployeeAdded ev)
        {
            if (!IsOffice(ev.OfficeId)) return;
            var office = GetOffice(ev.OfficeId);
            office.AddEmployee(ev.GlobalId);
        }

        private void Apply(EmployeeRemoved ev)
        {
            if (!IsOffice(ev.OfficeId)) return;
            var office = GetOffice(ev.OfficeId);
            office.RemoveEmployee(ev.Id);
        }

        private bool HasOfficeAccess(string employeeId, string officeId)
        {
            if (!IsOffice(officeId)) return false;

            return IsCompanyAdmin(employeeId) || IsOfficeAdmin(employeeId, officeId);
        }

        public bool HasAccessToAddEmployeeToOffice(string adminId, string officeId)
        {
            return HasOfficeAccess(adminId, officeId);
        }

        public void AddNewEmployeeToOffice(string officeId, Employee employee)
        {
            var office = GetOffice(officeId);
            var ev = new EmployeeAdded(_id, _name, office.Id, office.Name, employee.Id, employee.Name);

            ApplyChange(ev);
        }
    }
}
