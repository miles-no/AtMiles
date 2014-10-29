using System;
using System.Collections.Generic;

namespace no.miles.at.Backend.ReadStore.SearchStore
{
    public class EmployeeSearchModel
    {
        private Tag[] _competency =new List<Tag>().ToArray();
        private List<string> _keyQualifications = new List<string>();
        private List<BusyTime> _busyTimeEntries = new List<BusyTime>();
        private List<Description> _descriptions = new List<Description>();
        
        public string Id { get; set; }
        public string GlobalId { get; set; }
        public string CompanyId { get; set; }
        public string OfficeName { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
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
            get { return _competency; }
            set { _competency = value; }
        }

        public List<BusyTime> BusyTimeEntries
        {
            get { return _busyTimeEntries; }
            set { _busyTimeEntries = value; }
        }

        public List<string> KeyQualifications
        {
            get { return _keyQualifications; }
            set { _keyQualifications = value; }
        }

        public List<Description> Descriptions
        {
            get { return _descriptions; }
            set { _descriptions = value; }
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