using System;
using Newtonsoft.Json;

namespace no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Cv
{
    
    public class KeyQualification
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled;

        [JsonProperty(PropertyName = "int_long_description")]
        public string IntLongDescription;

        [JsonProperty(PropertyName = "int_tag_line")]
        public object IntTagLine;

        [JsonProperty(PropertyName = "key_points")]
        public KeyPoint[] KeyPoints;

        [JsonProperty(PropertyName = "local_long_description")]
        public string LocalLongDescription;

        [JsonProperty(PropertyName = "local_tag_line")]
        public object LocalTagLine;

        [JsonProperty(PropertyName = "long_description")]
        public object LongDescription;

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId;

        [JsonProperty(PropertyName = "order")]
        public int? Order;

        [JsonProperty(PropertyName = "starred")]
        public bool Starred;

        [JsonProperty(PropertyName = "tag_line")]
        public object TagLine;

        [JsonProperty(PropertyName = "version")]
        public int? Version;
    }

    public class KeyPoint
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "int_name")]
        public string IntName;

        [JsonProperty(PropertyName = "local_name")]
        public string LocalName;

        [JsonProperty(PropertyName = "int_long_description")]
        public string IntDescription;

        [JsonProperty(PropertyName = "local_long_description")]
        public string LocalDescription;

        [JsonProperty(PropertyName = "version")]
        public int? Version;
    }

    public class Language
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled;

        [JsonProperty(PropertyName = "int_level")]
        public string IntLevel;

        [JsonProperty(PropertyName = "int_name")]
        public string IntName;

        [JsonProperty(PropertyName = "level")]
        public string Level;

        [JsonProperty(PropertyName = "local_level")]
        public string LocalLevel;

        [JsonProperty(PropertyName = "local_name")]
        public string LocalName;

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId;

        [JsonProperty(PropertyName = "name")]
        public string Name;

        [JsonProperty(PropertyName = "order")]
        public int? Order;

        [JsonProperty(PropertyName = "starred")]
        public bool Starred;

        [JsonProperty(PropertyName = "version")]
        public int? Version;
    }

    public class Role
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled;

        [JsonProperty(PropertyName = "int_long_description")]
        public object IntLongDescription;

        [JsonProperty(PropertyName = "int_name")]
        public object IntName;

        [JsonProperty(PropertyName = "local_long_description")]
        public object LocalLongDescription;

        [JsonProperty(PropertyName = "local_name")]
        public object LocalName;

        [JsonProperty(PropertyName = "long_description")]
        public object LongDescription;

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId;

        [JsonProperty(PropertyName = "name")]
        public object Name;

        [JsonProperty(PropertyName = "order")]
        public int? Order;

        [JsonProperty(PropertyName = "starred")]
        public bool Starred;

        [JsonProperty(PropertyName = "version")]
        public int? Version;
    }

    public class ProjectExperience
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "customer")]
        public object Customer;

        [JsonProperty(PropertyName = "customer_description")]
        public object CustomerDescription;

        [JsonProperty(PropertyName = "customer_value_proposition")]
        public object CustomerValueProposition;

        [JsonProperty(PropertyName = "description")]
        public string Description;

        [JsonProperty(PropertyName = "disabled")]
        public bool? Disabled;

        [JsonProperty(PropertyName = "exclude_tags")]
        public object[] ExcludeTags;

        [JsonProperty(PropertyName = "expected_roll_off_date")]
        public object ExpectedRollOffDate;

        [JsonProperty(PropertyName = "industry")]
        public object Industry;

        [JsonProperty(PropertyName = "int_customer")]
        public object IntCustomer;

        [JsonProperty(PropertyName = "int_customer_description")]
        public object IntCustomerDescription;

        [JsonProperty(PropertyName = "int_customer_value_proposition")]
        public object IntCustomerValueProposition;

        [JsonProperty(PropertyName = "int_description")]
        public object IntDescription;

        [JsonProperty(PropertyName = "int_industry")]
        public object IntIndustry;

        [JsonProperty(PropertyName = "int_long_description")]
        public object IntLongDescription;

        [JsonProperty(PropertyName = "int_month_from")]
        public object IntMonthFrom;

        [JsonProperty(PropertyName = "int_month_to")]
        public object IntMonthTo;

        [JsonProperty(PropertyName = "int_related_work_experience_id")]
        public object IntRelatedWorkExperienceId;

        [JsonProperty(PropertyName = "int_tags")]
        public object[] IntTags;

        [JsonProperty(PropertyName = "int_year_from")]
        public object IntYearFrom;

        [JsonProperty(PropertyName = "int_year_to")]
        public object IntYearTo;

        [JsonProperty(PropertyName = "local_customer")]
        public object LocalCustomer;

        [JsonProperty(PropertyName = "local_customer_description")]
        public object LocalCustomerDescription;

        [JsonProperty(PropertyName = "local_customer_value_proposition")]
        public object LocalCustomerValueProposition;

        [JsonProperty(PropertyName = "local_description")]
        public object LocalDescription;

        [JsonProperty(PropertyName = "local_industry")]
        public object LocalIndustry;

        [JsonProperty(PropertyName = "local_long_description")]
        public object LocalLongDescription;

        [JsonProperty(PropertyName = "local_month_from")]
        public object LocalMonthFrom;

        [JsonProperty(PropertyName = "local_month_to")]
        public object LocalMonthTo;

        [JsonProperty(PropertyName = "local_related_work_experience_id")]
        public object LocalRelatedWorkExperienceId;

        [JsonProperty(PropertyName = "local_tags")]
        public object[] LocalTags;

        [JsonProperty(PropertyName = "local_year_from")]
        public object LocalYearFrom;

        [JsonProperty(PropertyName = "local_year_to")]
        public object LocalYearTo;

        [JsonProperty(PropertyName = "long_description")]
        public object LongDescription;

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId;

        [JsonProperty(PropertyName = "month_from")]
        public object MonthFrom;

        [JsonProperty(PropertyName = "month_to")]
        public object MonthTo;

        [JsonProperty(PropertyName = "order")]
        public int? Order;

        [JsonProperty(PropertyName = "related_work_experience_id")]
        public object RelatedWorkExperienceId;

        [JsonProperty(PropertyName = "roles")]
        public Role[] Roles;

        [JsonProperty(PropertyName = "starred")]
        public bool? Starred;

        [JsonProperty(PropertyName = "tags")]
        public object[] Tags;

        [JsonProperty(PropertyName = "version")]
        public int Version;

        [JsonProperty(PropertyName = "year_from")]
        public object YearFrom;

        [JsonProperty(PropertyName = "year_to")]
        public object YearTo;
    }

    public class Tags
    {

        [JsonProperty(PropertyName = "no")]
        public string No;

        [JsonProperty(PropertyName = "int")]
        public string Int;
    }

    public class TechnologySkill
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId;

        [JsonProperty(PropertyName = "order")]
        public int? Order;

        [JsonProperty(PropertyName = "tags")]
        public Tags Tags;

        [JsonProperty(PropertyName = "version")]
        public int? Version;
    }

    public class Technology
    {

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "category")]
        public string Category;

        [JsonProperty(PropertyName = "disabled")]
        public bool? Disabled;

        [JsonProperty(PropertyName = "exclude_tags")]
        public object[] ExcludeTags;

        [JsonProperty(PropertyName = "int_category")]
        public string IntCategory;

        [JsonProperty(PropertyName = "int_tags")]
        public string[] IntTags;

        [JsonProperty(PropertyName = "local_category")]
        public string LocalCategory;

        [JsonProperty(PropertyName = "local_tags")]
        public string[] LocalTags;

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId;

        [JsonProperty(PropertyName = "order")]
        public int? Order;

        [JsonProperty(PropertyName = "starred")]
        public object Starred;

        [JsonProperty(PropertyName = "tags")]
        public string[] Tags;

        [JsonProperty(PropertyName = "technology_skills")]
        public TechnologySkill[] TechnologySkills;

        [JsonProperty(PropertyName = "uncategorized")]
        public bool Uncategorized;

        [JsonProperty(PropertyName = "version")]
        public int? Version;
    }

    public class Thumb
    {

        [JsonProperty(PropertyName = "url")]
        public string Url;
    }

    public class FitThumb
    {

        [JsonProperty(PropertyName = "url")]
        public string Url;
    }

    public class SmallThumb
    {

        [JsonProperty(PropertyName = "url")]
        public string Url;
    }

    public class Image
    {

        [JsonProperty(PropertyName = "url")]
        public string Url;

        [JsonProperty(PropertyName = "thumb")]
        public Thumb Thumb;

        [JsonProperty(PropertyName = "fit_thumb")]
        public FitThumb FitThumb;

        [JsonProperty(PropertyName = "small_thumb")]
        public SmallThumb SmallThumb;
    }

    public class Cv
    {
// ReSharper disable once InconsistentNaming
        private int? bornYear;
// ReSharper disable once InconsistentNaming
        private int? bornMonth;
// ReSharper disable once InconsistentNaming
        private int? bornDay;

        [JsonProperty(PropertyName = "_id")]
        public string Id;

        [JsonProperty(PropertyName = "born_day")]
        public int? BornDay
        {
            get
            {
                if (bornDay == null || bornDay == 0)
                {
                    bornDay = 26;
                }
                return bornDay;
            }
            set { bornDay = value; }
        }

        [JsonProperty(PropertyName = "born_month")]
        public int? BornMonth
        {
            get
            {
                if (bornMonth == null || bornMonth == 0)
                {
                    bornMonth = 5;
                }
                return bornMonth;
            }
            set { bornMonth = value; }
        }

        [JsonProperty(PropertyName = "born_year")]
        public int? BornYear
        {
            get
            {
                if (bornYear == null || bornYear == 0)
                {
                    bornYear = 1926;
                }
                
                return bornYear;
            }
            set { bornYear = value; }
        }

        [JsonProperty(PropertyName = "bruker_id")]
        public string UserId;

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt;

        [JsonProperty(PropertyName = "default")]
        public bool Default;

        [JsonProperty(PropertyName = "default_international")]
        public bool DefaultInternational;

        [JsonProperty(PropertyName = "int_nationality")]
        public object IntNationality;

        [JsonProperty(PropertyName = "int_place_of_residence")]
        public object IntPlaceOfResidence;

        [JsonProperty(PropertyName = "int_title")]
        public object IntTitle;

        [JsonProperty(PropertyName = "key_qualifications")]
        public KeyQualification[] KeyQualifications;

        [JsonProperty(PropertyName = "language_code")]
        public string LanguageCode;

        [JsonProperty(PropertyName = "languages")]
        public Language[] Languages;

        [JsonProperty(PropertyName = "local_nationality")]
        public string LocalNationality;

        [JsonProperty(PropertyName = "local_place_of_residence")]
        public string LocalPlaceOfResidence;

        [JsonProperty(PropertyName = "local_title")]
        public string LocalTitle;

        [JsonProperty(PropertyName = "modifier_id")]
        public object ModifierId;

        [JsonProperty(PropertyName = "nationality")]
        public string Nationality;

        [JsonProperty(PropertyName = "navn")]
        public string Name;

        [JsonProperty(PropertyName = "place_of_residence")]
        public string PlaceOfResidence;

        [JsonProperty(PropertyName = "project_experiences")]
        public ProjectExperience[] ProjectExperiences;

        [JsonProperty(PropertyName = "technologies")]
        public Technology[] Technologies;

        [JsonProperty(PropertyName = "telefon")]
        public string Phone;

        [JsonProperty(PropertyName = "tilbud_id")]
        public object TilbudId;

        [JsonProperty(PropertyName = "title")]
        public string Title;

        [JsonProperty(PropertyName = "twitter")]
        public object Twitter;

        [JsonProperty(PropertyName = "updated_at")]
        public DateTime UpdatedAt;

        [JsonProperty(PropertyName = "version")]
        public int? Version;

        [JsonProperty(PropertyName = "email")]
        public string Email;

        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode;

        [JsonProperty(PropertyName = "language_codes")]
        public string[] LanguageCodes;

        [JsonProperty(PropertyName = "image")]
        public Image Image;

        [JsonProperty(PropertyName = "can_write")]
        public bool CanWrite;
    }



}