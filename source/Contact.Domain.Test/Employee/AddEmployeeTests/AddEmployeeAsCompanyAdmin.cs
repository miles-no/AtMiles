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
    public class AddEmployeeAsCompanyAdmin : EventSpecification<AddEmployee>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private DateTime _timestamp1 = DateTime.MinValue;
        private DateTime _timestamp2 = DateTime.MinValue;
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeId = "SVG";
        private const string OfficeName = "Stavanger";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";
        private static readonly DateTime AdminDateOfBirth = new DateTime(1980, 01, 01);

        private readonly string employeeGlobalId = new Guid().ToString();
        private readonly Login employeeLoginId = new Login("Google", "mail@miles.no", "google:123456789");

        private const string EmployeeFirstName = "Kurt";
        private const string EmployeeLastName = "Kurtson";
        private static readonly DateTime EmployeeDateOfBirth = new DateTime(2000, 01, 01);

        [Test]
        public void add_employee_as_company_admin()
        {
            Setup();
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
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName , AdminLastName), DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                    
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, AdminDateOfBirth, string.Empty,string.Empty,string.Empty,null,null, null,DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                };
            return events;
        }

        public override AddEmployee When()
        {
            var cmd = new AddEmployee(CompanyId, OfficeId, employeeGlobalId, employeeLoginId, EmployeeFirstName, string.Empty, EmployeeLastName, EmployeeDateOfBirth, string.Empty, string.Empty, string.Empty, null, null, null, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, 2);
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
                    new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, employeeGlobalId, employeeLoginId, EmployeeFirstName, string.Empty, EmployeeLastName, EmployeeDateOfBirth,string.Empty,string.Empty,string.Empty,null,null, null, _timestamp1,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId),
                    new EmployeeAdded(CompanyId, CompanyName, OfficeId, OfficeName, employeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), _timestamp2,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
