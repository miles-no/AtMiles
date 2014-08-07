using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Events
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

        public EmployeeCreated(string companyId, string companyName, string officeId, string officeName, string globalId, string firstName, string middleName, string lastName, DateTime dateOfBirth)
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
        }

        public EmployeeCreated(string companyId, string companyName, string officeId, string officeName, string globalId, string firstName, string lastName, DateTime dateOfBirth)
            : this(companyId, companyName, officeId, officeName, globalId, firstName, string.Empty, lastName, dateOfBirth) { }

         public EmployeeCreated WithJobTitle(string jobTitle)
        {
            JobTitle = jobTitle;
            return this;
        }

         public EmployeeCreated WithPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            return this;
        }

         public EmployeeCreated WithEmail(string email)
        {
            Email = email;
            return this;
        }

         public EmployeeCreated WithPhoto(Picture photo)
        {
            Photo = photo;
            return this;
        }

         public EmployeeCreated WithHomeAddress(Address homeAddress)
        {
            HomeAddress = homeAddress;
            return this;
        }
    }
}
