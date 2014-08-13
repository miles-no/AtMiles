using System;

namespace Contact.Backend.Models.Api.Queries
{
    public class EmployeeFulltextSearchResponse
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
    }
}