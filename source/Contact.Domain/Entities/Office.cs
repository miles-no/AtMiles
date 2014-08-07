using System.Collections.Generic;
using System.Linq;

namespace Contact.Domain.Entities
{
    public class Office
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        private readonly List<string> _officeAdmins;
        private readonly List<string> _employees;

        public Office(string id, string name)
        {
            Id = id;
            Name = name;
            _officeAdmins = new List<string>();
            _employees = new List<string>();
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

        public void AddEmployee(string employeeId)
        {
            if (!_employees.Contains(employeeId))
            {
                _employees.Add(employeeId);
            }
        }

        public void RemoveEmployee(string employeeId)
        {
            _employees.RemoveAll(item => item == employeeId);
        }

        public bool IsEmptyForEmployees()
        {
            return _employees.Count == 0;
        }
    }
}
