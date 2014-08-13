using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class EmployeeCreated : Event
    {
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string OfficeId { get; private set; }
        public string OfficeName { get; private set; }
        public string GlobalId { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string JobTitle { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public Address HomeAddress { get; private set; }
        public Picture Photo { get; private set; }

        public EmployeeCreated(string companyId, string companyName, string officeId, string officeName, string globalId,
            string firstName, string middleName, string lastName,
            DateTime dateOfBirth, string jobTitle, string phoneNumber, string email,
            Picture photo, Address homeAddress,
            DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            OfficeId = officeId;
            OfficeName = officeName;
            GlobalId = globalId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            JobTitle = jobTitle;
            PhoneNumber = phoneNumber;
            Email = email;
            Photo = photo;
            HomeAddress = homeAddress;
        }
    }
}
