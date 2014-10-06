package no.miles.atmiles.employee;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonTypeInfo;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SearchResultModel {
    private int _total;

    @JsonProperty("Total")
    public int getTotal(){
        return _total;
    }

    @JsonProperty("Total")
    public void setTotal(int total) {
        _total = total;
    }

    private int _skipped;

    @JsonProperty("Skipped")
    public int getSkipped(){
        return _skipped;
    }

    @JsonProperty("Skipped")
    public void setSkipped(int skipped) {
        _skipped = skipped;
    }

    private Result[] _results;

    @JsonProperty("Results")
    public Result[] getResults(){
        return _results;
    }

    @JsonProperty("Results")
    //@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, include = JsonTypeInfo.As.PROPERTY, property = Result)
    public void setResults(Result[] results){
        _results = results;
    }

    public class Result{
        private String _companyId;

        @JsonProperty("CompanyId")
        public String getCompanyId(){
            return _companyId;
        }

        @JsonProperty("CompanyId")
        public void setCompanyId(String value){
            _companyId = value;
        }
        private String _officeName;

        @JsonProperty("OfficeName")
        public String getOfficeName(){
            return _officeName;
        }

        @JsonProperty("OfficeName")
        public void setOfficeName(String value){
            _officeName = value;
        }

        private String _globalId;

        @JsonProperty("GlobalId")
        public String getGlobalId(){
            return _globalId;
        }

        @JsonProperty("GlobalId")
        public void setGlobalId(String value){
            _globalId = value;
        }

        private String _name;

        @JsonProperty("Name")
        public String getName(){
            return _name;
        }

        @JsonProperty("Name")
        public void setName(String value){
            _name = value;
        }

        private String _dateOfBirth;

        @JsonProperty("DateOfBirth")
        public String getDateOfBirth(){
            return _dateOfBirth;
        }

        @JsonProperty("DateOfBirth")
        public void setDateOfBirth(String value){
            _dateOfBirth = value;
        }

        private String _jobTitle;

        @JsonProperty("JobTitle")
        public String getJobTitle(){
            return _jobTitle;
        }

        @JsonProperty("JobTitle")
        public void setJobTitle(String value){
            _jobTitle = value;
        }

        private String _phoneNumber;

        @JsonProperty("PhoneNumber")
        public String getPhoneNumber(){
            return _phoneNumber;
        }

        @JsonProperty("PhoneNumber")
        public void setPhoneNumber(String value){
            _phoneNumber = value;
        }

        private String _email;

        @JsonProperty("Email")
        public String getEmail(){
            return _email;
        }

        @JsonProperty("Email")
        public void setEmail(String value){
            _email = value;
        }

        private String _thumb;

        @JsonProperty("Thumb")
        public String getThumb(){
            return _thumb;
        }

        @JsonProperty("Thumb")
        public void setThumb(String value){
            _thumb = value;
        }

        private Tag[] _competency;

        @JsonProperty("Competency")
        public Tag[] getCompetency(){
            return _competency;
        }

        @JsonProperty("Competency")
        public void setCompetency(Tag[] value){
            _competency = value;
        }
    }

    public class Tag{
        private String _category;

        @JsonProperty("Category")
        public String getCategory(){
            return _category;
        }

        @JsonProperty("Category")
        public void setCategory(String value){
            _category = value;
        }

        private String _competency;

        @JsonProperty("Competency")
        public String getCompetency(){
            return _competency;
        }

        @JsonProperty("Competency")
        public void setCompetency(String value){
            _competency = value;
        }

        private String _internationalCategory;

        @JsonProperty("InternationalCategory")
        public String getInternationalCategory(){
            return _internationalCategory;
        }

        @JsonProperty("InternationalCategory")
        public void setInternationalCategory(String value){
            _internationalCategory = value;
        }

        private String _internationalCompetency;

        @JsonProperty("InternationalCompetency")
        public String getInternationalCompetency(){
            return _internationalCompetency;
        }

        @JsonProperty("InternationalCompetency")
        public void setInternationalCompetency(String value){
            _internationalCompetency = value;
        }
    }
}


