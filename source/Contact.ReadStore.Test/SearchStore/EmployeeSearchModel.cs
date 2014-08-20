using System;

namespace Contact.ReadStore.Test.SearchStore
{
    public class EmployeeSearchModel
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Thumb { get; set; }
        public Tag[] Competency { get; set; }
        public double Score { get; set; }
    }
    
   
}