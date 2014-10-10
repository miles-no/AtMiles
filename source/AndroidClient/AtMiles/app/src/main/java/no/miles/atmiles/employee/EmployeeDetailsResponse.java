package no.miles.atmiles.employee;

public class EmployeeDetailsResponse {
    public String Id;
    public String GlobalId;
    public String CompanyId;
    public String OfficeName;
    public String Name;
    public String DateOfBirth;
    public String JobTitle;
    public String PhoneNumber;
    public String Email;
    public Address PrivateAddress;
    public String Thumb;
    public Tag[] Competency;
    public BusyTime[] BusyTimeEntries;
    public String[] KeyQualifications;
    public Description[] Descriptions;
    public double Score;

    public static class Tag{
        public String Category;
        public String Competency;
        public String InternationalCompentency;
        public String InternationalCategory;
    }

    public static class BusyTime
    {
        public String Id;
        public String Start;
        public String End;
        public int PercentageOccupied;
        public String Comment;
    }

    public static class Description
    {
        public String InternationalDescription;
        public String LocalDescription;
    }

    public static class Address {
        public String Street;
        public String PostalCode;
        public String PostalName;
    }
}
