﻿using System;
using System.Collections.Generic;
using System.Linq;
using no.miles.at.Backend.Domain.Annotations;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Aggregates
{
    public class Company : AggregateRoot
    {
        private readonly List<string> _companyAdmins;
        private readonly List<EmployeeLoginInfo> _employees;
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public Company()
        {
            _name = string.Empty;
            _companyAdmins = new List<string>();
            _employees = new List<EmployeeLoginInfo>();
        }

        public bool IsCompanyAdmin(string identifier)
        {
            return _companyAdmins.Contains(identifier);
        }

        private bool HasUser(Login login)
        {
            if (login == null) return false;
            return _employees.Any(u => u.LoginId.Provider == login.Provider && u.LoginId.Email == login.Email);
        }

        private string GetUserId(Login login)
        {
            if (login == null) return string.Empty;
            if (!string.IsNullOrEmpty(login.Email))
            {
                var employeeLoginInfo =
                    _employees.FirstOrDefault(
                        u => u.LoginId.Provider == login.Provider && u.LoginId.Email == login.Email);
                return employeeLoginInfo != null ? employeeLoginInfo.Id : string.Empty;
            }

            return string.Empty;
        }

        public string GetUserIdByLoginId(Login login)
        {
            if (HasUser(login))
            {
                return GetUserId(login);
            }

            //Not found
            return string.Empty;
        }

        public void CreateNewCompany(string companyId, string companyName, string adminId, string adminName, DateTime created, Person createdBy, string correlationId)
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
            if (IsCompanyAdmin(employeeToBeAdmin.Id)) throw new AlreadyExistingItemException("Already company admin");

            var ev = new CompanyAdminAdded(
                companyId: Id,
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
            if (OnlyOneCompanyAdminLeft()) throw new LastItemException("Cannot remove last admin");
            if (employeeToBeRemoved.Id == createdBy.Identifier) throw new NoAccessException("Cannot remove self");

            var ev = new CompanyAdminRemoved(
                companyId: Id,
                companyName: _name,
                adminId: employeeToBeRemoved.Id,
                adminName: employeeToBeRemoved.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }


        public void AddNewEmployeeToCompany(Employee employee, Person createdBy, string correlationId)
        {
            var ev = new EmployeeAdded(
                companyId: Id,
                companyName: _name,
                globalId: employee.Id,
                name: employee.Name,
                login: employee.LoginId,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void RemoveEmployee(Employee employee, Person createdBy, string correlationId)
        {
            var ev = new EmployeeRemoved(
                companyId: Id,
                companyName: _name,
                id: employee.Id,
                name: employee.Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public IEnumerable<string> GetAllUserIdsForUsersNotInList(IEnumerable<CvPartnerImportData> importData)
        {
            return (from employee in _employees
                    where importData.All(i => i.Email != employee.LoginId.Email)
                    where employee.Id != Constants.SystemUserId
                    select employee.Id)
                    .ToList();
        }

        private bool OnlyOneCompanyAdminLeft()
        {
            return _companyAdmins.Count == 1;
        }

        private void AddEmployee(EmployeeLoginInfo employeeInfo)
        {
            if (_employees.All(e => e.Id != employeeInfo.Id))
            {
                _employees.Add(employeeInfo);
            }
        }

        private void RemoveEmployee(string employeeId)
        {
            _employees.RemoveAll(item => item.Id == employeeId);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CompanyCreated ev)
        {
            Id = ev.CompanyId;
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
        private void Apply(EmployeeAdded ev)
        {
            AddEmployee(new EmployeeLoginInfo(ev.GlobalId, ev.Login));
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeRemoved ev)
        {
            RemoveEmployee(ev.Id);
        }
    }
}
