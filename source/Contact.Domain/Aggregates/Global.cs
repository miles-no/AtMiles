using System;
using System.Collections.Generic;
using Contact.Domain.Annotations;
using Contact.Domain.Events.Global;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Aggregates
{
    public class Global : AggregateRoot
    {
        public const string GlobalId = "GLOBAL";

        private readonly List<string> _companies;

        public string[] Companies
        {
            get { return _companies.ToArray(); }
        }

        public Global()
        {
            _id = GlobalId;
            _companies = new List<string>();
        }

        public void AddCompany(Company company, Person createdBy, string correlationId)
        {
            var ev = new CompanyCreated(company.Id, company.Name, DateTime.UtcNow, createdBy, correlationId);
            ApplyChange(ev);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(CompanyCreated ev)
        {
            _companies.Add(ev.Id);
        }
    }
}
