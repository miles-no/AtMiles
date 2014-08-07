using System.Collections.Generic;
using System.Linq;

namespace Contact.Domain.Entities
{
    public class Office
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        private readonly List<string> _officeAdmins; 

        public Office(string id, string name)
        {
            Id = id;
            Name = name;
            _officeAdmins = new List<string>();
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
    }
}
