using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Employee.AddEmployeeTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class AddEmployeeAsCompanyAdminTest : EventSpecification<AddEmployee>
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

        private readonly string _employeeGlobalId = new Guid().ToString();
        private readonly Login _employeeLoginId = new Login(Constants.GoogleIdProvider, "mail@miles.no");

        private const string EmployeeFirstName = "Kurt";
        private const string EmployeeLastName = "Kurtson";

        [Test]
        public async void add_employee_as_company_admin()
        {
            await Setup();
        }

        public override IEnumerable<Event> Produced()
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

        public IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName , AdminLastName), DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                    
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                };
            return events;
        }

        public override AddEmployee When()
        {
            var cmd = new AddEmployee(CompanyId, _employeeGlobalId, _employeeLoginId, EmployeeFirstName, string.Empty, EmployeeLastName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, 2);
            return cmd;
        }

        public override Handles<AddEmployee> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        public override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new EmployeeCreated(CompanyId, CompanyName, _employeeGlobalId, _employeeLoginId, EmployeeFirstName, string.Empty, EmployeeLastName, _timestamp1, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId),
                    new EmployeeAdded(CompanyId, CompanyName, _employeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), _employeeLoginId, _timestamp2, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
