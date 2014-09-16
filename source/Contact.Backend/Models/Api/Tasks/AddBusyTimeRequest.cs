using System;

namespace Contact.Backend.Models.Api.Tasks
{
    public class AddBusyTimeRequest
    {
        public string CompanyId { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public short PercentageOccupied { get; set; }
        public string Comment { get; set; }
    }
}