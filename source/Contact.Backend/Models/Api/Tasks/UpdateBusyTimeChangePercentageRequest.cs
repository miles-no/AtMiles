namespace Contact.Backend.Models.Api.Tasks
{
    public class UpdateBusyTimeChangePercentageRequest
    {
        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }

        public short NewPercentageOccupied { get; set; }
    }
}