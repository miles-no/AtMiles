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

        public AddEmployee(string companyId, string officeId, string globalId, string firstName, string middleName, string lastName, DateTime dateOfBirth)
        {
            CompanyId = companyId;
            OfficeId = officeId;
            GlobalId = globalId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        public AddEmployee(string companyId, string officeId, string globalId, string firstName, string lastName, DateTime dateOfBirth)
            : this(companyId, officeId, globalId, firstName, string.Empty, lastName, dateOfBirth) { }

        public AddEmployee WithJobTitle(string jobTitle)
        {
            JobTitle = jobTitle;
            return this;
        }

        public AddEmployee WithPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            return this;
        }

        public AddEmployee WithEmail(string email)
        {
            Email = email;
            return this;
        }

        public AddEmployee WithPhoto(Picture photo)
        {
            Photo = photo;
            return this;
        }

        public AddEmployee WithHomeAddress(Address homeAddress)
        {
            HomeAddress = homeAddress;
            return this;
        }
    }
}
