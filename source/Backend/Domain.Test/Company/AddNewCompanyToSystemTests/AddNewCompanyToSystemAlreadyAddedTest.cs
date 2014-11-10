using System;
using System.Collections.Generic;
using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Events.Global;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.ValueTypes;
using NUnit.Framework;

namespace no.miles.at.Backend.Domain.Test.Company.AddNewCompanyToSystemTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class AddNewCompanyToSystemAlreadyAddedTest : EventSpecification<AddNewCompanyToSystem>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();

        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeRepository<Global> _fakeGlobalRepository;
        private FakeCvPartnerImporter _fakeImporter;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

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

        protected override IEnumerable<Event> Produced()
        {
            yield break;
        }

        protected override IEnumerable<FakeStreamEvent> Given()
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
                    new FakeStreamEvent(Global.GlobalId, new CompanyCreated(CompanyId, CompanyName,new DateTime(2014,1,1),new Person(Constants.SystemUserId, Constants.SystemUserId), "SYSTEM INIT" ))
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

        protected override AddNewCompanyToSystem When()
        {
            var admins = new List<SimpleUserInfo>
            {
                new SimpleUserInfo(AdminId, AdminFirstName, string.Empty, AdminLastName,
                    new Login(Constants.GoogleIdProvider, AdminEmail))
            };
            return new AddNewCompanyToSystem(CompanyId, CompanyName, admins.ToArray(), DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId), _correlationId, Constants.IgnoreVersion);
        }

        protected override Handles<AddNewCompanyToSystem> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeGlobalRepository = new FakeRepository<Global>(GivenGlobal());
            _fakeImporter = new FakeCvPartnerImporter(new List<CvPartnerImportData>());
            return new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeGlobalRepository, _fakeImporter, new FakeEnrichFromAuth0());
        }

        protected override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
