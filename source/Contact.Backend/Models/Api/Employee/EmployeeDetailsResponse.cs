using System;
using System.Collections.Generic;
using Contact.ReadStore.SearchStore;

namespace Contact.Backend.Models.Api.Employee
{
    public class EmployeeDetailsResponse
    {
        public string Id { get; set; }
        public string GlobalId { get; set; }
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Thumb { get; set; }
        
        public Tag[] Competency { get; set; }
        
        public List<BusyTime> BusyTimeEntries { get; set; }
        
        public List<string> KeyQualifications { get; set; }
        
        public List<Description> Descriptions { get; set; }
        
        public double Score { get; set; }

        public class BusyTime
        {
            public string Id { get; set; }
            public DateTime Start { get; set; }
            public DateTime? End { get; set; }
            public short PercentageOccupied { get; set; }
            public string Comment { get; set; }
        }

        public class Description
        {
            public string InternationalDescription { get; set; }
            public string LocalDescription { get; set; }
        } 
    }
}