using Contact.Domain.Events;
using Contact.Domain.Services;

namespace Contact.Domain.Aggregates
{
    public class Employee : AggregateRoot
    {
        private string _firstName;
        private string _middleName;
        private string _lastName;

        public string Name
        {
            get { return NameService.GetName(_firstName, _middleName, _lastName); }
        }

        private void Apply(EmployeeCreated ev)
        {
            _id = ev.GlobalId;
            _firstName = ev.FirstName;
            _middleName = ev.MiddleName;
            _lastName = ev.LastName;
        }

        private void Apply(EmployeeTerminated ev)
        {
        }
    }
}
