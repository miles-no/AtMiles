using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Import
{
    public class EnrichedFromAuth0 : Event
    {
        public string CompanyId { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Etag { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }

        public EnrichedFromAuth0(string companyId, string employeeId, string firstName, string lastName, string etag, string email, string phone, string photo, DateTime created, Person createdBy, string correlationId)
            : base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
            FirstName = firstName;
            LastName = lastName;
            Etag = etag;
            Email = email;
            Phone = phone;
            Photo = photo;
        }
    }
}