using Newtonsoft.Json;

namespace no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Employee
{
    public class Thumb
    {

        [JsonProperty("url")]
        public string Url;
    }

    public class FitThumb
    {

        [JsonProperty("url")]
        public string Url;
    }

    public class SmallThumb
    {

        [JsonProperty("url")]
        public string Url;
    }

    public class Image
    {

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("thumb")]
        public Thumb Thumb;

        [JsonProperty("fit_thumb")]
        public FitThumb FitThumb;

        [JsonProperty("small_thumb")]
        public SmallThumb SmallThumb;
    }

    public class Employee
    {

        [JsonProperty("user_id")]
        public string UserId;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("email")]
        public string Email;

        [JsonProperty("telephone")]
        public string Telephone;

        [JsonProperty("role")]
        public string Role;

        [JsonProperty("office_id")]
        public string OfficeId;

        [JsonProperty("office_name")]
        public string OfficeName;

        [JsonProperty("country_id")]
        public string CountryId;

        [JsonProperty("country_code")]
        public string CountryCode;

        [JsonProperty("international_toggle")]
        public string InternationalToggle;

        [JsonProperty("preferred_download_format")]
        public string PreferredDownloadFormat;

        [JsonProperty("masterdata_languages")]
        public string[] MasterdataLanguages;

        [JsonProperty("expand_proposals_toggle")]
        public bool ExpandProposalsToggle;

        [JsonProperty("default_cv_id")]
        public string DefaultCvId;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("image")]
        public Image Image;
    }

}