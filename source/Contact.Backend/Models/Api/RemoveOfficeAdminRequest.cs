﻿namespace Contact.Backend.Models.Api
{
    public class RemoveOfficeAdminRequest
    {
        public string CompanyId { get; set; }
        public string AdminId { get; set; }
        public string OfficeId { get; set; }
    }
}