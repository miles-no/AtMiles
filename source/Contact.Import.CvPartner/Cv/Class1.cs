using Newtonsoft.Json;

namespace Contact.Import.CvPartner.Cv
{
    
    public class KeyQualification
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled { get; set; }

        [JsonProperty(PropertyName = "int_long_description")]
        public object IntLongDescription { get; set; }

        [JsonProperty(PropertyName = "int_tag_line")]
        public object IntTagLine { get; set; }

        [JsonProperty(PropertyName = "local_long_description")]
        public object LocalLongDescription { get; set; }

        [JsonProperty(PropertyName = "local_tag_line")]
        public object LocalTagLine { get; set; }

        [JsonProperty(PropertyName = "long_description")]
        public object LongDescription { get; set; }

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }

        [JsonProperty(PropertyName = "starred")]
        public bool Starred { get; set; }

        [JsonProperty(PropertyName = "tag_line")]
        public object TagLine { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }
    }

    public class Language
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled { get; set; }

        [JsonProperty(PropertyName = "int_level")]
        public string IntLevel { get; set; }

        [JsonProperty(PropertyName = "int_name")]
        public string IntName { get; set; }

        [JsonProperty(PropertyName = "level")]
        public string Level { get; set; }

        [JsonProperty(PropertyName = "local_level")]
        public string LocalLevel { get; set; }

        [JsonProperty(PropertyName = "local_name")]
        public string LocalName { get; set; }

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }

        [JsonProperty(PropertyName = "starred")]
        public bool Starred { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }
    }

    public class Role
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled { get; set; }

        [JsonProperty(PropertyName = "int_long_description")]
        public object IntLongDescription { get; set; }

        [JsonProperty(PropertyName = "int_name")]
        public object IntName { get; set; }

        [JsonProperty(PropertyName = "local_long_description")]
        public object LocalLongDescription { get; set; }

        [JsonProperty(PropertyName = "local_name")]
        public object LocalName { get; set; }

        [JsonProperty(PropertyName = "long_description")]
        public object LongDescription { get; set; }

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public object Name { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }

        [JsonProperty(PropertyName = "starred")]
        public bool Starred { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }
    }

    public class ProjectExperience
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public object Customer { get; set; }

        [JsonProperty(PropertyName = "customer_description")]
        public object CustomerDescription { get; set; }

        [JsonProperty(PropertyName = "customer_value_proposition")]
        public object CustomerValueProposition { get; set; }

        [JsonProperty(PropertyName = "description")]
        public object Description { get; set; }

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled { get; set; }

        [JsonProperty(PropertyName = "exclude_tags")]
        public object[] ExcludeTags { get; set; }

        [JsonProperty(PropertyName = "expected_roll_off_date")]
        public object ExpectedRollOffDate { get; set; }

        [JsonProperty(PropertyName = "industry")]
        public object Industry { get; set; }

        [JsonProperty(PropertyName = "int_customer")]
        public object IntCustomer { get; set; }

        [JsonProperty(PropertyName = "int_customer_description")]
        public object IntCustomerDescription { get; set; }

        [JsonProperty(PropertyName = "int_customer_value_proposition")]
        public object IntCustomerValueProposition { get; set; }

        [JsonProperty(PropertyName = "int_description")]
        public object IntDescription { get; set; }

        [JsonProperty(PropertyName = "int_industry")]
        public object IntIndustry { get; set; }

        [JsonProperty(PropertyName = "int_long_description")]
        public object IntLongDescription { get; set; }

        [JsonProperty(PropertyName = "int_month_from")]
        public object IntMonthFrom { get; set; }

        [JsonProperty(PropertyName = "int_month_to")]
        public object IntMonthTo { get; set; }

        [JsonProperty(PropertyName = "int_related_work_experience_id")]
        public object IntRelatedWorkExperienceId { get; set; }

        [JsonProperty(PropertyName = "int_tags")]
        public object[] IntTags { get; set; }

        [JsonProperty(PropertyName = "int_year_from")]
        public object IntYearFrom { get; set; }

        [JsonProperty(PropertyName = "int_year_to")]
        public object IntYearTo { get; set; }

        [JsonProperty(PropertyName = "local_customer")]
        public object LocalCustomer { get; set; }

        [JsonProperty(PropertyName = "local_customer_description")]
        public object LocalCustomerDescription { get; set; }

        [JsonProperty(PropertyName = "local_customer_value_proposition")]
        public object LocalCustomerValueProposition { get; set; }

        [JsonProperty(PropertyName = "local_description")]
        public object LocalDescription { get; set; }

        [JsonProperty(PropertyName = "local_industry")]
        public object LocalIndustry { get; set; }

        [JsonProperty(PropertyName = "local_long_description")]
        public object LocalLongDescription { get; set; }

        [JsonProperty(PropertyName = "local_month_from")]
        public object LocalMonthFrom { get; set; }

        [JsonProperty(PropertyName = "local_month_to")]
        public object LocalMonthTo { get; set; }

        [JsonProperty(PropertyName = "local_related_work_experience_id")]
        public object LocalRelatedWorkExperienceId { get; set; }

        [JsonProperty(PropertyName = "local_tags")]
        public object[] LocalTags { get; set; }

        [JsonProperty(PropertyName = "local_year_from")]
        public object LocalYearFrom { get; set; }

        [JsonProperty(PropertyName = "local_year_to")]
        public object LocalYearTo { get; set; }

        [JsonProperty(PropertyName = "long_description")]
        public object LongDescription { get; set; }

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId { get; set; }

        [JsonProperty(PropertyName = "month_from")]
        public object MonthFrom { get; set; }

        [JsonProperty(PropertyName = "month_to")]
        public object MonthTo { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }

        [JsonProperty(PropertyName = "related_work_experience_id")]
        public object RelatedWorkExperienceId { get; set; }

        [JsonProperty(PropertyName = "roles")]
        public Role[] Roles { get; set; }

        [JsonProperty(PropertyName = "starred")]
        public bool Starred { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public object[] Tags { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "year_from")]
        public object YearFrom { get; set; }

        [JsonProperty(PropertyName = "year_to")]
        public object YearTo { get; set; }
    }

    public class Tags
    {

        [JsonProperty(PropertyName = "no")]
        public string No { get; set; }

        [JsonProperty(PropertyName = "int")]
        public string Int { get; set; }
    }

    public class TechnologySkill
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public Tags Tags { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }
    }

    public class Technology
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "disabled")]
        public bool? Disabled { get; set; }

        [JsonProperty(PropertyName = "exclude_tags")]
        public object[] ExcludeTags { get; set; }

        [JsonProperty(PropertyName = "int_category")]
        public string IntCategory { get; set; }

        [JsonProperty(PropertyName = "int_tags")]
        public string[] IntTags { get; set; }

        [JsonProperty(PropertyName = "local_category")]
        public string LocalCategory { get; set; }

        [JsonProperty(PropertyName = "local_tags")]
        public string[] LocalTags { get; set; }

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId { get; set; }

        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }

        [JsonProperty(PropertyName = "starred")]
        public object Starred { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public string[] Tags { get; set; }

        [JsonProperty(PropertyName = "technology_skills")]
        public TechnologySkill[] TechnologySkills { get; set; }

        [JsonProperty(PropertyName = "uncategorized")]
        public bool Uncategorized { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }
    }

    public class Thumb
    {

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public class FitThumb
    {

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public class SmallThumb
    {

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public class Image
    {

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "thumb")]
        public Thumb Thumb { get; set; }

        [JsonProperty(PropertyName = "fit_thumb")]
        public FitThumb FitThumb { get; set; }

        [JsonProperty(PropertyName = "small_thumb")]
        public SmallThumb SmallThumb { get; set; }
    }

    public class Cv
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "born_day")]
        public int BornDay { get; set; }

        [JsonProperty(PropertyName = "born_month")]
        public int BornMonth { get; set; }

        [JsonProperty(PropertyName = "born_year")]
        public int BornYear { get; set; }

        [JsonProperty(PropertyName = "bruker_id")]
        public string BrukerId { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "default")]
        public bool Default { get; set; }

        [JsonProperty(PropertyName = "default_international")]
        public bool DefaultInternational { get; set; }

        [JsonProperty(PropertyName = "int_nationality")]
        public object IntNationality { get; set; }

        [JsonProperty(PropertyName = "int_place_of_residence")]
        public object IntPlaceOfResidence { get; set; }

        [JsonProperty(PropertyName = "int_title")]
        public object IntTitle { get; set; }

        [JsonProperty(PropertyName = "key_qualifications")]
        public KeyQualification[] KeyQualifications { get; set; }

        [JsonProperty(PropertyName = "language_code")]
        public string LanguageCode { get; set; }

        [JsonProperty(PropertyName = "languages")]
        public Language[] Languages { get; set; }

        [JsonProperty(PropertyName = "local_nationality")]
        public string LocalNationality { get; set; }

        [JsonProperty(PropertyName = "local_place_of_residence")]
        public string LocalPlaceOfResidence { get; set; }

        [JsonProperty(PropertyName = "local_title")]
        public string LocalTitle { get; set; }

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId { get; set; }

        [JsonProperty(PropertyName = "nationality")]
        public string Nationality { get; set; }

        [JsonProperty(PropertyName = "navn")]
        public string Navn { get; set; }

        [JsonProperty(PropertyName = "place_of_residence")]
        public string PlaceOfResidence { get; set; }

        [JsonProperty(PropertyName = "project_experiences")]
        public ProjectExperience[] ProjectExperiences { get; set; }

        [JsonProperty(PropertyName = "technologies")]
        public Technology[] Technologies { get; set; }

        [JsonProperty(PropertyName = "telefon")]
        public string Telefon { get; set; }

        [JsonProperty(PropertyName = "tilbud_id")]
        public object TilbudId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "twitter")]
        public object Twitter { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "language_codes")]
        public string[] LanguageCodes { get; set; }

        [JsonProperty(PropertyName = "image")]
        public Image Image { get; set; }

        [JsonProperty(PropertyName = "can_write")]
        public bool CanWrite { get; set; }
    }



}