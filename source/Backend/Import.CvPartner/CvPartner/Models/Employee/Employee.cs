using Newtonsoft.Json;

namespace Contact.Import.CvPartner.CvPartner.Models.Employee
{
    public class Thumb
    {

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class FitThumb
    {

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class SmallThumb
    {

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Image
    {

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("thumb")]
        public Thumb Thumb { get; set; }

        [JsonProperty("fit_thumb")]
        public FitThumb FitThumb { get; set; }

        [JsonProperty("small_thumb")]
        public SmallThumb SmallThumb { get; set; }
    }

    public class Employee
    {

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("office_id")]
        public string OfficeId { get; set; }

        [JsonProperty("office_name")]
        public string OfficeName { get; set; }

        [JsonProperty("country_id")]
        public string CountryId { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("international_toggle")]
        public string InternationalToggle { get; set; }

        [JsonProperty("preferred_download_format")]
        public string PreferredDownloadFormat { get; set; }

        [JsonProperty("masterdata_languages")]
        public string[] MasterdataLanguages { get; set; }

        [JsonProperty("expand_proposals_toggle")]
        public bool ExpandProposalsToggle { get; set; }

        [JsonProperty("default_cv_id")]
        public string DefaultCvId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }
    }

}