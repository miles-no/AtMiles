using System;

namespace Contact.Backend.Models
{
    public class Employee
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string GlobalId { get; set; }
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

    public class Address
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string PostalName { get; set; }
    }

    public class Picture
    {
        public string Title { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }
    }
}