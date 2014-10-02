using System;
using System.Collections.Generic;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company.AddNewCompanyToSystemTests
{
    [TestFixture]
    public class AddNewCompanyToSystemAlreadyAddedTest : EventSpecification<AddNewCompanyToSystem>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();

        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeRepository<Global> _fakeGlobalRepository;
        private FakeCvPartnerImporter _fakeImporter;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeName = "Stavanger";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";
        private const string AdminEmail = "admin@miles.no";

        [Test]
        public async void add_new_company_to_system_already_added()
        {
            ExpectedException = new AlreadyExistingItemException("CompanyId already in system");
            await Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            yield break;
        }

        public override IEnumerable<FakeStreamEvent> Given()
        {
            var c = GivenCompany();
            var e = GivenEmployee();
            var g = GivenGlobal();
            var all = new List<FakeStreamEvent>(c);
            all.AddRange(e);
            all.AddRange(g);
            return all;
        }

        private IEnumerable<FakeStreamEvent> GivenGlobal()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(Global.GlobalId, new Events.Global.CompanyCreated(CompanyId, CompanyName,new DateTime(2014,1,1),new Person(Constants.SystemUserId, Constants.SystemUserId), "SYSTEM INIT" ))
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            yield break;
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            yield break;
        }

        public override AddNewCompanyToSystem When()
        {
            var admins = new List<SimpleUserInfo>
            {
                new SimpleUserInfo(AdminId, AdminFirstName, string.Empty, AdminLastName,
                    new Login(Constants.GoogleIdProvider, AdminEmail))
            };
            return new AddNewCompanyToSystem(CompanyId, CompanyName, OfficeName, admins.ToArray(), DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId), _correlationId, Constants.IgnoreVersion);
        }

        public override Handles<AddNewCompanyToSystem> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeGlobalRepository = new FakeRepository<Global>(GivenGlobal());
            _fakeImporter = new FakeCvPartnerImporter(new List<CvPartnerImportData>());
            return new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeGlobalRepository, _fakeImporter);
        }

        public override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
