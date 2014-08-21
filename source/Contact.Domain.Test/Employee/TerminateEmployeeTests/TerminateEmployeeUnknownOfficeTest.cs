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

namespace Contact.Domain.Test.Employee.TerminateEmployeeTests
{
    [TestFixture]
    public class TerminateEmployeeUnknownOfficeTest : EventSpecification<TerminateEmployee>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeCvPartnerImporter _fakeCvPartnerImporter;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeId = "SVG";
        private const string OfficeName = "Stavanger";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";
        private static readonly DateTime AdminDateOfBirth = new DateTime(1980, 01, 01);

        private const string EmployeeGlobalId = "google:mail@miles.no";
        private const string EmployeeFirstName = "Kurt";
        private const string EmployeeLastName = "Kurtson";
        private static readonly DateTime EmployeeDateOfBirth = new DateTime(2000, 01, 01);

        [Test]
        public void terminate_employee_unknown_office()
        {
            ExpectedException = new UnknownItemException(string.Empty);
            Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            var events1 = _fakeEmployeeRepository.GetThenEvents();
            var events2 = _fakeCompanyRepository.GetThenEvents();
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
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), null, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId))
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, AdminDateOfBirth, string.Empty,string.Empty,string.Empty,null,null, null,DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    new FakeStreamEvent(EmployeeGlobalId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeGlobalId, null, EmployeeFirstName, string.Empty, EmployeeLastName, EmployeeDateOfBirth, string.Empty,string.Empty,string.Empty,null,null, null,DateTime.UtcNow,new Person(EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName)), _correlationId)),
                };
            return events;
        }

        public override TerminateEmployee When()
        {
            var cmd = new TerminateEmployee(CompanyId, OfficeId, EmployeeGlobalId, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, 2);
            return cmd;
        }

        public override Handles<TerminateEmployee> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeCvPartnerImporter = new FakeCvPartnerImporter();
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeCvPartnerImporter);
        }

        public override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new EmployeeTerminated(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId),
                    new EmployeeRemoved(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeGlobalId, NameService.GetName(EmployeeFirstName, EmployeeLastName), DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
