using System.Collections.Generic;
using System.Linq;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Entities
{
    public class Office
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        private readonly List<string> _officeAdmins;
        private readonly List<EmployeeLoginInfo> _employees;

        public Office(string id, string name)
        {
            Id = id;
            Name = name;
            _officeAdmins = new List<string>();
            _employees = new List<EmployeeLoginInfo>();
            if (Id == null) Id = string.Empty;
        }

        public void AddAdmin(string adminId)
        {
            if (!_officeAdmins.Contains(adminId))
            {
                _officeAdmins.Add(adminId);
            }
        }

        public void RemoveAdmin(string adminId)
        {
            _officeAdmins.RemoveAll(item => item == adminId);
        }

        public bool IsAdmin(string identifier)
        {
            return _officeAdmins.Contains(identifier);
        }

        public void AddEmployee(EmployeeLoginInfo employeeInfo)
        {
            if (_employees.All(e => e.Id != employeeInfo.Id))
            {
                _employees.Add(employeeInfo);
            }
        }

        public void RemoveEmployee(string employeeId)
        {
            _employees.RemoveAll(item => item.Id == employeeId);
        }

        public bool IsEmptyForEmployees()
        {
            return _employees.Count == 0;
        }

        public bool HasUser(Login login)
        {
            if (login == null) return false;

            if (!string.IsNullOrEmpty(login.Id))
            {
                return _employees.Any(u => u.LoginId.Provider == login.Provider && u.LoginId.Id == login.Id);
            }
            else
            {
                return _employees.Any(u => u.LoginId.Provider == login.Provider && u.LoginId.Email == login.Email);
            }
        }

        public string GetUserId(Login login)
        {
            if (login == null) return string.Empty;
            if (!string.IsNullOrEmpty(login.Id))
            {
                var employeeLoginInfo = _employees.FirstOrDefault(u => u.LoginId.Provider == login.Provider && u.LoginId.Id == login.Id);
                return employeeLoginInfo != null ? employeeLoginInfo.Id : string.Empty;
            }
            else
            {
                var employeeLoginInfo = _employees.FirstOrDefault(u => u.LoginId.Provider == login.Provider && u.LoginId.Email == login.Email);
                return employeeLoginInfo != null ? employeeLoginInfo.Id : string.Empty;
            }
        }
    }
}
