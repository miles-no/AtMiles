package no.miles.atmiles.employee;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonTypeInfo;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SearchResultModel {
    public int Total;
    public int Skipped;
    public Result[] Results;

    public static class Result{
        public String CompanyId;
        public String OfficeName;
        public String GlobalId;
        public String Name;
        public String DateOfBirth;
        public String JobTitle;
        public String PhoneNumber;
        public String Email;
        public String Thumb;
        public Tag[] Competency;

        public static class Tag {
            public String Category;
            public String Competency;
            public String InternationalCategory;
            public String InternationalCompentency;
        }
    }
}


