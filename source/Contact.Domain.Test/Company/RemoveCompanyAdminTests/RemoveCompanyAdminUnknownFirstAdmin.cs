using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Exceptions;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company.RemoveCompanyAdminTests
{
    [TestFixture]
    public class RemoveCompanyAdminUnknownFirstAdmin : EventSpecification<RemoveCompanyAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeCvPartnerImporter _fakeCvPartnerImporter;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string ExistingAdminId = "old1";
        private const string ExistingAdminFirstName = "Existing";
        private const string ExistingAdminLastName = "Admin";

        private const string NewAdminId = "new1";
        private const string NewAdminFirstName = "New";
        private const string NewAdminLastName = "Admin";
        private static readonly DateTime NewAdminDateOfBirth = new DateTime(1981, 01, 01);

        private const string OfficeId = "office1";
        private const string OfficeName = "Stavanger";

        [Test]
        public void remove_company_admin_unknown_first_admin()
        {
            ExpectedException = new UnknownItemException(string.Empty);
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
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null, DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, NewAdminId, NameService.GetName(NewAdminFirstName, NewAdminLastName), DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)),_correlationId)),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(NewAdminId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, NewAdminId, null, NewAdminFirstName, string.Empty, NewAdminLastName, NewAdminDateOfBirth, string.Empty,string.Empty,string.Empty,null,null, null,DateTime.UtcNow,new Person(NewAdminId, NameService.GetName(NewAdminFirstName, NewAdminLastName)), _correlationId)),
                };
            return events;
        }

        public override RemoveCompanyAdmin When()
        {
            var cmd = new RemoveCompanyAdmin(CompanyId, NewAdminId, DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)), _correlationId, 2);
            return cmd;
        }

        public override Handles<RemoveCompanyAdmin> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeCvPartnerImporter = new FakeCvPartnerImporter();
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeCvPartnerImporter);
        }

        public override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
