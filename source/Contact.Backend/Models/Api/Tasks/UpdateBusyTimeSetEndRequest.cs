using System;

namespace Contact.Backend.Models.Api.Tasks
{
    public class UpdateBusyTimeSetEndRequest
    {
        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }
        public DateTime? NewEnd { get; set; }
    }
}