using System;
using System.Collections.Generic;
using Contact.Domain.Annotations;
using Contact.Domain.Entities;
using System.Linq;
using Contact.Domain.Events.Company;
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

        public bool HasOfficeAdminAccess(string employeeId, string officeId)
        {
            if (!IsOffice(officeId)) return false;

            return IsCompanyAdmin(employeeId) || IsOfficeAdmin(employeeId, officeId);
        }

        public string GetUserIdByLoginId(Login login)
        {
            foreach (var office in _offices)
            {
                if (office.HasUser(login))
                {
                    return office.GetUserId(login);
                }
            }

            //Not found
            return string.Empty;
        }

        public Office GetOfficeByName(string officeName)
        {
            return _offices.FirstOrDefault(o => o.Name == officeName);
        }

        public void CreateNewCompany(string companyId, string companyName, string officeId, string officeName, Address officeAddress, string adminId, string adminName, DateTime created, Person createdBy, string correlationId)
        {
            var ev1 = new CompanyCreated(
                companyId: companyId,
                companyName: companyName,
                created: created,
                createdBy: createdBy,
                correlationId: correlationId);

            var ev2 = new CompanyAdminAdded(
                companyId: companyId,
                companyName: companyName,
                newAdminId: adminId,
                newAdminName: adminName,
                created: created,
                createdBy: createdBy,
                correlationId: correlationId);

            ApplyChange(ev1);
            ApplyChange(ev2);
        }

        public void AddCompanyAdmin(Employee employeeToBeAdmin, Person createdBy, string correlationId)
        {
            if(IsCompanyAdmin(employeeToBeAdmin.Id)) throw new AlreadyExistingItemException("Already company admin");

            var ev = new CompanyAdminAdded(
                companyId: _id,
                companyName: _name,
                newAdminId: employeeToBeAdmin.Id,
                newAdminName: employeeToBeAdmin.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void RemoveCompanyAdmin(Employee employeeToBeRemoved, Person createdBy, string correlationId)
        {
            if(OnlyOneCompanyAdminLeft()) throw new LastItemException("Cannot remove last admin");
            if (employeeToBeRemoved.Id == createdBy.Identifier) throw new NoAccessException("Cannot remove self");

            var ev = new CompanyAdminRemoved(
                companyId: _id,
                companyName: _name,
                adminId: employeeToBeRemoved.Id,
                adminName: employeeToBeRemoved.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void OpenOffice(string officeId, string officeName, Address address, Person createdBy, string correlationId)
        {
            var ev = new OfficeOpened(
                companyId: _id,
                companyName: _name,
                officeId: officeId,
                officeName: officeName,
                address: address,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void CloseOffice(string officeId, Person createdBy, string correlationId)
        {
            if (OnlyOneOfficeLeft()) throw new LastItemException("Cannot close last office");

            var office = GetOffice(officeId);

            if(!office.IsEmptyForEmployees()) throw new ExistingChildItemsException("Cannot close non-empty office");

            var ev = new OfficeClosed(
                companyId: _id,
                companyName: _name,
                officeId: office.Id,
                officeName: office.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void AddOfficeAdmin(string officeId, Employee newAdmin, Person createdBy, string correlationId)
        {
            if(!IsOffice(officeId)) throw new UnknownItemException("Unknown ID for Office");
            
            var office = GetOffice(officeId);

            if (!HasOfficeAdminAccess(createdBy.Identifier, office.Id)) throw new NoAccessException("No access to complete this operation");

            if(office.IsAdmin(newAdmin.Id)) throw new AlreadyExistingItemException("Already admin in this office");

            var ev = new OfficeAdminAdded(
                companyId: _id,
                companyName: _name,
                officeId: office.Id,
                officeName: office.Name,
                adminId: newAdmin.Id,
                adminName: newAdmin.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void RemoveOfficeAdmin(string officeId, Employee adminToBeRemoved, Person createdBy, string correlationId)
        {
            if (!IsOffice(officeId)) throw new UnknownItemException("Unknown ID for Office");

            var office = GetOffice(officeId);

            if (!HasOfficeAdminAccess(createdBy.Identifier, office.Id)) throw new NoAccessException("No access to complete this operation");
            
            CheckIfTryingToDisconnectSelf(adminToBeRemoved, createdBy);

            var ev = new OfficeAdminRemoved(
                companyId: _id,
                companyName: _name,
                officeId: office.Id,
                officeName: office.Name,
                adminId: adminToBeRemoved.Id,
                adminName: adminToBeRemoved.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void AddNewEmployeeToOffice(string officeId, Employee employee, Person createdBy, string correlationId)
        {
            var office = GetOffice(officeId);
            var ev = new EmployeeAdded(
                companyId: _id,
                companyName: _name,
                officeId: office.Id,
                officeName: office.Name,
                globalId: employee.Id,
                name: employee.Name,
                login: employee.LoginId,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void RemoveEmployee(string officeId, Employee employee, Person createdBy, string correlationId)
        {
            var office = GetOffice(officeId);
            var ev = new EmployeeRemoved(
                companyId: _id,
                companyName: _name,
                officeId: office.Id,
                officeName: office.Name,
                id: employee.Id,
                name: employee.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void MoveEmployeeToNewOffice(Employee employee, string oldOfficeId, string newOfficeId, Person createdBy, string correlationId)
        {
            var oldOffice = GetOffice(oldOfficeId);
            var newOffice = GetOffice(newOfficeId);

            if(!oldOffice.HasUser(employee.Id)) throw new UnknownItemException("Employee not found in Office to move from.");

            var ev = new EmployeeMovedToNewOffice(
                companyId: _id,
                companyName: Name,
                oldOfficeId: oldOffice.Id,
                oldOfficeName: oldOffice.Name,
                newOfficeId: newOffice.Id,
                newOfficeName: newOffice.Name,
                employeeId: employee.Id,
                employeeName: employee.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        private bool OnlyOneCompanyAdminLeft()
        {
            return _companyAdmins.Count == 1;
        }

        private void CheckIfTryingToDisconnectSelf(Employee adminToBeRemoved, Person createdBy)
        {
            if (adminToBeRemoved.Id == createdBy.Identifier)
            {
                if (!IsCompanyAdmin(adminToBeRemoved.Id)) throw new NoAccessException("Cannot remove self");
            }
        }

        private bool OnlyOneOfficeLeft()
        {
            return _offices.Count == 1;
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CompanyCreated ev)
        {
            _id = ev.CompanyId;
            _name = ev.CompanyName;
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CompanyAdminAdded ev)
        {
            if (_companyAdmins.Contains(ev.NewAdminId)) return;

            _companyAdmins.Add(ev.NewAdminId);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CompanyAdminRemoved ev)
        {
            _companyAdmins.RemoveAll(item => item == ev.AdminId);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(OfficeOpened ev)
        {
            if (IsOffice(ev.OfficeId)) return;

            var office = new Office(ev.OfficeId, ev.OfficeName);
            _offices.Add(office);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(OfficeClosed ev)
        {
            if (!IsOffice(ev.OfficeId)) return;

            _offices.RemoveAll(item => item.Id == ev.OfficeId);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(OfficeAdminAdded ev)
        {
            if (!IsOffice(ev.OfficeId)) return;

            var office = GetOffice(ev.OfficeId);
            office.AddAdmin(ev.AdminId);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(OfficeAdminRemoved ev)
        {
            if (!IsOffice(ev.OfficeId)) return;
            var office = GetOffice(ev.OfficeId);
            office.RemoveAdmin(ev.AdminId);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeAdded ev)
        {
            if (!IsOffice(ev.OfficeId)) return;
            var office = GetOffice(ev.OfficeId);
            office.AddEmployee(new EmployeeLoginInfo(ev.GlobalId, ev.Login));
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeRemoved ev)
        {
            if (!IsOffice(ev.OfficeId)) return;
            var office = GetOffice(ev.OfficeId);
            office.RemoveEmployee(ev.Id);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeMovedToNewOffice ev)
        {
            var oldOffice = GetOffice(ev.OldOfficeId);
            var newOffice = GetOffice(ev.NewOfficeId);
            var employee = oldOffice.GetEmployee(ev.EmployeeId);
            oldOffice.RemoveEmployee(ev.EmployeeId);
            newOffice.AddEmployee(employee);
        }
    }
}
