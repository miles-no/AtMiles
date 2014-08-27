using System;
using System.Collections.Generic;

namespace Contact.ReadStore.SearchStore
{
    public class EmployeeSearchModel
    {
        private Tag[] competency =new List<Tag>().ToArray();
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

        public Tag[] Competency
        {
            get { return competency; }
            set { competency = value; }
        }

        public double Score { get; set; }
    }
    
   
}