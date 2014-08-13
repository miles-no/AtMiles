using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddEmployee : Command
    {
        public string CompanyId { get; private set; }
        public string OfficeId { get; private set; }
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
