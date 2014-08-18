using System;
using Contact.Domain.ValueTypes;

namespace Contact.TestApp.InMemoryReadModel
{
    public class Employee
    {
        public SimpleCompany Company { get; set; }
        public SimpleOffice Office { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Address HomeAddress { get; set; }
        public Picture Photo { get; set; }
    }
}
