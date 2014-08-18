using System;
using System.Linq;

namespace Contact.ReadStore.Test
{
    public class PersonSearchModel
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Thumb { get; set; }
        public string Query { get
        {
            return Name + " " + OfficeId + " " + " " + JobTitle + " " + Email + " " +
                   (Competency != null ? string.Join(" ",Competency.Select(s => s.Category + " " + s.Competency))  : string.Empty);
        } }
        public Tag[] Competency { get; set; }
    }
}