namespace Contact.Backend.Models.Api
{
    public class ResponseBase
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class AsyncResponseBase : ResponseBase
    {
        public string Url { get; set; }
    }
}