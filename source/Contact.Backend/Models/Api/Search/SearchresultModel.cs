using System;
using System.Collections.Generic;

namespace Contact.Backend.Models.Api.Search
{
    public class SearchRequestModel
    {
        public string Query { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class SearchResultModel
    {
        public int Total { get; set; }
        public int Skipped { get; set; }
        public List<Result> Results { get; set; }
    }

    public class Result
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
    public class Tag
    {
        public string Category { get; set; }
        public string Competency { get; set; }
        public string InternationalCompentency { get; set; }
        public string InternationalCategory { get; set; }
    }
}