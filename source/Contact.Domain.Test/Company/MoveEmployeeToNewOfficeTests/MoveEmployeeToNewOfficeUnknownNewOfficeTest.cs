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

namespace Contact.Domain.Test.Company.MoveEmployeeToNewOfficeTests
{
    [TestFixture]
    public class MoveEmployeeToNewOfficeUnknownNewOfficeTest : EventSpecification<MoveEmployeeToNewOffice>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OldOfficeId = "BGN";
        private const string OldOfficeName = "Bergen";

        private const string NewOfficeId = "SVG";
        private const string NewOfficeName = "Stavanger";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";
        private static readonly DateTime AdminDateOfBirth = new DateTime(1980, 01, 01);

        private const string EmployeeId = "emp1";
        private const string EmployeeFirstName = "Admin";
        private const string EmployeeLastName = "Adminson";
        private static readonly DateTime EmployeeDateOfBirth = new DateTime(1980, 01, 01);

        [Test]
        public void move_employee_to_new_office_unknown_new_office()
        {
            ExpectedException = new UnknownItemException("Unknown ID for Office to move to.");
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
            var admin = new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName));
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, admin,"INIT")),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OldOfficeId, OldOfficeName, null, DateTime.UtcNow, admin,"INIT")),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName , AdminLastName), DateTime.UtcNow, admin,"INIT")),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, OldOfficeId, OldOfficeName, EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName),null,DateTime.UtcNow, admin, "INIT"))
                    
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, OldOfficeId, OldOfficeName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, AdminDateOfBirth, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), "INIT")),
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, OldOfficeId, OldOfficeName, EmployeeId, null, EmployeeFirstName, string.Empty, EmployeeLastName, EmployeeDateOfBirth, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), "INIT")),
                };
            return events;
        }

        public override MoveEmployeeToNewOffice When()
        {
            return new MoveEmployeeToNewOffice(CompanyId, OldOfficeId, NewOfficeId, EmployeeId, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, Constants.IgnoreVersion);
        }

        public override Handles<MoveEmployeeToNewOffice> OnHandler()
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
