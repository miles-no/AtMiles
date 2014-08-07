using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events;
using Contact.Domain.Exceptions;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company
{
    public class OpenOfficeWithoutPermisionTest : EventSpecification<OpenOffice>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string companyId = "miles";
        private const string companyName = "Miles";

        private const string existingOfficeId = "bgn";
        private const string existingOfficeName = "Bergen";

        private const string officeName = "Stavanger";

        private const string adminId = "adm1";
        private const string adminFirstName = "Admin";
        private const string adminLastName = "Adminson";
        private static readonly DateTime adminDateOfBirth = new DateTime(1980, 01, 01);

        [Test]
        public void open_office_without_permission()
        {
            ExpectedException = new NoAccessException();
            Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            var events = _fakeCompanyRepository.GetThenEvents();
            return events;
        }

        public override IEnumerable<FakeStreamEvent> Given()
        {
            var c = GivenCompany();
            var e = GivenEmployee();
            var all = new List<FakeStreamEvent>(c);
            all.AddRange(e);
            return all;
        }

        public IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(companyId, new CompanyCreated(companyId, companyName)),
                    new FakeStreamEvent(companyId, new OfficeOpened(companyId, companyName, existingOfficeId, existingOfficeName, null))
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(adminId, new EmployeeCreated(companyId, companyName, existingOfficeId, existingOfficeName, adminId, adminFirstName, adminLastName, adminDateOfBirth)),
                };
            return events;
        }

        public override OpenOffice When()
        {
            var cmd = new OpenOffice(companyId, officeName)
                .WithCreated(DateTime.UtcNow)
                .WithCorrelationId(_correlationId)
                .WithBasedOnVersion(2)
                .WithCreatedBy(new Person(adminId, NameService.GetName(adminFirstName, adminLastName)));
            return (OpenOffice)cmd;
        }

        public override Handles<OpenOffice> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        public override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
