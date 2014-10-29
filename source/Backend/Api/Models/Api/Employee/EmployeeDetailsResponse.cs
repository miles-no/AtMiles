using System;
using System.Collections.Generic;

namespace no.miles.at.Backend.Api.Models.Api.Employee
{
    public class EmployeeDetailsResponse
    {
        public string Id;
        public string GlobalId;
        public string CompanyId;
        public string OfficeName;
        public string Name;
        public DateTime DateOfBirth;
        public string JobTitle;
        public string PhoneNumber;
        public string Email;
        public Address PrivateAddress;
        public string Thumb;

        public Tag[] Competency;

        public List<BusyTime> BusyTimeEntries;

        public List<string> KeyQualifications;

        public List<Description> Descriptions;

        public double Score;

        public class BusyTime
        {
            public string Id;
            public DateTime Start;
            public DateTime? End;
            public short PercentageOccupied;
            public string Comment;
        }

        public class Description
        {
            public string InternationalDescription;
            public string LocalDescription;
        }

        public class Address
        {
            public string Street;
            public string PostalCode;
            public string PostalName;
        }

        public class Tag
        {
            public string Category;
            public string Competency;
            public string InternationalCompentency;
            public string InternationalCategory;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}