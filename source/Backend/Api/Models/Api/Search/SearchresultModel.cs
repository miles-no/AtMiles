using System;
using System.Collections.Generic;

namespace no.miles.at.Backend.Api.Models.Api.Search
{
    public class SearchRequestModel
    {
        public string Query;
        public int Skip;
        public int Take;
    }

    public class SearchResultModel
    {
        public int Total;
        public int Skipped;
        public List<Result> Results;
    }

    public class Result
    {
        public string CompanyId;
        public string OfficeName;
        public string GlobalId;
        public string Name;
        public DateTime DateOfBirth;
        public string JobTitle;
        public string PhoneNumber;
        public string Email;
        public string Thumb;
        public Address PrivateAddress;

        public class Address
        {
            public string Street;
            public string PostalCode;
            public string PostalName;
        }
    }
}