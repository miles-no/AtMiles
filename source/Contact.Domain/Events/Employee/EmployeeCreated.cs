using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events.Employee
{
    public class EmployeeCreated : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly Login LoginId;
        public readonly string FirstName;
        public readonly string MiddleName;
        public readonly string LastName;
        public readonly DateTime? DateOfBirth;
        public readonly string JobTitle;
        public readonly string OfficeName;
        public readonly string PhoneNumber;
        public readonly string Email;
        public readonly Address HomeAddress;
        public readonly Picture Photo;

        public EmployeeCreated(string companyId, string companyName, string employeeId,
            Login loginId, string firstName, string middleName, string lastName,
            DateTime? dateOfBirth, string jobTitle, string officeName, string phoneNumber, string email,
            Address homeAddress, Picture photo, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            LoginId = loginId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            JobTitle = jobTitle;
            OfficeName = officeName;
            PhoneNumber = phoneNumber;
            Email = email;
            HomeAddress = homeAddress;
            Photo = photo;
        }
    }
}
