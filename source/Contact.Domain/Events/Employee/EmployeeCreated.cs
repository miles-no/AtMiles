using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class EmployeeCreated : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string OfficeId;
        public readonly string OfficeName;
        public readonly string GlobalId;
        public readonly string FirstName;
        public readonly string MiddleName;
        public readonly string LastName;
        public readonly DateTime DateOfBirth;
        public readonly string JobTitle;
        public readonly string PhoneNumber;
        public readonly string Email;
        public readonly Address HomeAddress;
        public readonly Picture Photo;

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
