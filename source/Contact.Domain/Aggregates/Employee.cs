using System;
using Contact.Domain.Annotations;
using Contact.Domain.Events.Employee;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;

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

        public void CreateNew(string companyId, string companyName, string officeId, string officeName, string globalId, string firstName, string middleName, string lastName, DateTime dateOfBirth, string jobTitle, string phoneNumber, string email, Address homeAddress, Picture photo, Person createdBy, string correlationId)
        {
            var ev = new EmployeeCreated(companyId, companyName, officeId, officeName, globalId, firstName, middleName,
                lastName, dateOfBirth,jobTitle,phoneNumber,email,photo,homeAddress,DateTime.UtcNow,createdBy,correlationId);
            ApplyChange(ev);
        }

        public void Terminate(string companyId, string companyName, string officeId, string officeName, Person createdBy, string correlationId)
        {
            var ev = new EmployeeTerminated(companyId, companyName, officeId, officeName, _id,
                NameService.GetName(_firstName, _middleName, _lastName),DateTime.UtcNow, createdBy, correlationId);
            ApplyChange(ev);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeCreated ev)
        {
            _id = ev.GlobalId;
            _firstName = ev.FirstName;
            _middleName = ev.MiddleName;
            _lastName = ev.LastName;
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeTerminated ev)
        {
            //Empty for now. Might be used for soft delete later.
        }
    }
}
