using System;
using Contact.Domain.Events;
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
                lastName, dateOfBirth)
                .WithEmail(email)
                .WithHomeAddress(homeAddress)
                .WithJobTitle(jobTitle)
                .WithPhoneNumber(phoneNumber)
                .WithPhoto(photo)
                .WithCreated(DateTime.UtcNow)
                .WithCreatedBy(createdBy)
                .WithCorrelationId(correlationId);

            ApplyChange(ev);
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
