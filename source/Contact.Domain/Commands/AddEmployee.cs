using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddEmployee : Command
    {
        public readonly string CompanyId;
        public readonly string OfficeId;
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

        public AddEmployee(string companyId, string officeId, string globalId, string firstName, string middleName, string lastName, DateTime dateOfBirth, string jobTitle, string phoneNumber, string email, Address homeAddress, Picture photo, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            :base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            GlobalId = globalId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            JobTitle = jobTitle;
            PhoneNumber = phoneNumber;
            Email = email;
            HomeAddress = homeAddress;
            Photo = photo;
        }

        public AddEmployee(string companyId, string officeId, string globalId, string firstName, string lastName, DateTime dateOfBirth, string jobTitle, string phoneNumber, string email, Address homeAddress, Picture photo, DateTime created, Person createdBy, string correlationId, Int32 basedOnVersion)
            : this(companyId, officeId, globalId, firstName, string.Empty, lastName, dateOfBirth, jobTitle, phoneNumber, email, homeAddress, photo, created, createdBy, correlationId, basedOnVersion) { }
    }
}
