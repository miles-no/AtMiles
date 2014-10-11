using System;
using System.Collections.Generic;

namespace no.miles.at.Backend.ReadStore.SearchStore
{
    public class EmployeeSearchModel
    {
        private Tag[] competency =new List<Tag>().ToArray();
        private List<string> keyQualifications = new List<string>();
        private List<BusyTime> busyTimeEntries = new List<BusyTime>();
        private List<Description> descriptions = new List<Description>();
        
        public string Id { get; set; }
        public string GlobalId { get; set; }
        public string CompanyId { get; set; }
        public string OfficeName { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Thumb { get; set; }
        public DateTime BusyTimeEntriesConfirmed { get; set; }
        public Address PrivateAddress { get; set; }
        public bool DateOfBirthSetManually { get; set; }

        public Tag[] Competency
        {
            get { return competency; }
            set { competency = value; }
        }

        public List<BusyTime> BusyTimeEntries
        {
            get { return busyTimeEntries; }
            set { busyTimeEntries = value; }
        }

        public List<string> KeyQualifications
        {
            get { return keyQualifications; }
            set { keyQualifications = value; }
        }

        public List<Description> Descriptions
        {
            get { return descriptions; }
            set { descriptions = value; }
        }

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

        public class Address
        {
            public string Street { get; set; }
            public string PostalCode { get; set; }
            public string PostalName { get; set; }
        }
    }
    
   
}