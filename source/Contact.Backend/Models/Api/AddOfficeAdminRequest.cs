﻿namespace Contact.Backend.Models.Api
{
    public class AddOfficeAdminRequest
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string AdminId { get; set; }
    }
}