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
    public class RemoveCompanyAdminLastAdminTest : EventSpecification<RemoveCompanyAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string ExistingAdminId = "old1";
        private const string ExistingAdminFirstName = "Existing";
        private const string ExistingAdminLastName = "Admin";
        private static readonly DateTime ExistingAdminDateOfBirth = new DateTime(1980, 01, 01);

        private const string OfficeId = "office1";
        private const string OfficeName = "Stavanger";

        [Test]
        public void remove_company_admin_last_admin()
        {
            ExpectedException = new LastItemException(string.Empty);
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
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName), DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)),_correlationId)),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(ExistingAdminId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, ExistingAdminId, null, ExistingAdminFirstName, string.Empty, ExistingAdminLastName, ExistingAdminDateOfBirth, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)), _correlationId)),
                };
            return events;
        }

        public override RemoveCompanyAdmin When()
        {
            var cmd = new RemoveCompanyAdmin(CompanyId, ExistingAdminId, DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)), _correlationId, 2);
            return cmd;
        }

        public override Handles<RemoveCompanyAdmin> OnHandler()
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
