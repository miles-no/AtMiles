using System;
using System.Collections.Generic;

namespace Contact.Domain.ValueTypes
{
    public class CvPartnerImportData
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public string OfficeName { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CvPartnerKeyQualification[] KeyQualifications { get; set; }
        public CvPartnerTechnology[] Technologies { get; set; }
        public Picture Photo { get; set; }
    }
}
