using System;
using System.Collections.Generic;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using NUnit.Framework;

namespace no.miles.at.Backend.Domain.Test.Employee.TerminateEmployeeTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class TerminateEmployeeAsCompanyAdminTest : EventSpecification<TerminateEmployee>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private DateTime _timestamp1 = DateTime.MinValue;
        private DateTime _timestamp2 = DateTime.MinValue;
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";

        private const string EmployeeGlobalId = "google:mail@miles.no";
        private const string EmployeeFirstName = "Kurt";
        private const string EmployeeLastName = "Kurtson";

        [Test]
        public async void terminate_employee_as_company_admin()
        {
            await Setup();
        }

        protected override IEnumerable<Event> Produced()
        {
            var events1 = _fakeEmployeeRepository.GetThenEvents();
            if (events1.Count == 1)
            {
                _timestamp1 = events1[0].Created;
            }
            var events2 = _fakeCompanyRepository.GetThenEvents();
            if (events2.Count == 1)
            {
                _timestamp2 = events2[0].Created;
            }
            events1.AddRange(events2);
            return events1;
        }

        public override IEnumerable<FakeStreamEvent> Given()
        {
            var c = GivenCompany();
            var e = GivenEmployee();
            var all = new List<FakeStreamEvent>(c);
            all.AddRange(e);
            return all;
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName , AdminLastName), DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), null, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId))
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    new FakeStreamEvent(EmployeeGlobalId, new EmployeeCreated(CompanyId, CompanyName, EmployeeGlobalId, null, EmployeeFirstName, string.Empty, EmployeeLastName, DateTime.UtcNow, new Person(EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName)), _correlationId)),
                };
            return events;
        }

        protected override TerminateEmployee When()
        {
            var cmd = new TerminateEmployee(CompanyId, EmployeeGlobalId, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, 2);
            return cmd;
        }

        protected override Handles<TerminateEmployee> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        protected override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new EmployeeTerminated(CompanyId, CompanyName, EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), _timestamp1,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId),
                    new EmployeeRemoved(CompanyId, CompanyName, EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), _timestamp2,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
