﻿using System;
using System.Collections.Generic;
using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using NUnit.Framework;

namespace no.miles.at.Backend.Domain.Test.Company.AddNewCompanyToSystemTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class AddNewCompanyToSystemEmptySystemTest : EventSpecification<AddNewCompanyToSystem>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();

        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeRepository<Global> _fakeGlobalRepository;
        private FakeCvPartnerImporter _fakeImporter;
        private DateTime _timestamp11 = DateTime.MinValue;
        private DateTime _timestamp21 = DateTime.MinValue;
        private DateTime _timestamp22 = DateTime.MinValue;
        private DateTime _timestamp31 = DateTime.MinValue;
        private DateTime _timestamp32 = DateTime.MinValue;
        private DateTime _timestamp33 = DateTime.MinValue;
        private DateTime _timestamp34 = DateTime.MinValue;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";
        private const string AdminEmail = "admin@miles.no";

        [Test]
        public async void add_new_company_to_system_empty_system()
        {
            await Setup();
        }

        protected override IEnumerable<Event> Produced()
        {
            var events1 = _fakeGlobalRepository.GetThenEvents();
            if (events1.Count == 1)
            {
                _timestamp11 = events1[0].Created;
            }

            var events2 = _fakeEmployeeRepository.GetThenEvents();
            if (events2.Count == 2)
            {
                _timestamp21 = events2[0].Created;
                _timestamp22 = events2[1].Created;
            }

            var events3 = _fakeCompanyRepository.GetThenEvents();
            if (events3.Count == 4)
            {
                _timestamp31 = events3[0].Created;
                _timestamp32 = events3[1].Created;
                _timestamp33 = events3[2].Created;
                _timestamp34 = events3[3].Created;
            }

            events1.AddRange(events2);
            events1.AddRange(events3);
            return events1;
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

        protected override AddNewCompanyToSystem When()
        {
            var admins = new List<SimpleUserInfo>
            {
                new SimpleUserInfo(AdminId, AdminFirstName, string.Empty, AdminLastName,
                    new Login(Constants.GoogleIdProvider, AdminEmail))
            };
            return new AddNewCompanyToSystem(CompanyId, CompanyName, admins.ToArray(),DateTime.UtcNow,new Person(Constants.SystemUserId, Constants.SystemUserId), _correlationId, Constants.IgnoreVersion);
        }

        protected override Handles<AddNewCompanyToSystem> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeGlobalRepository = new FakeRepository<Global>(GivenGlobal());
            _fakeImporter = new FakeCvPartnerImporter(new List<CvPartnerImportData>());
            return new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeGlobalRepository,_fakeImporter, new FakeEnrichFromAuth0());
        }

        private IEnumerable<FakeStreamEvent> GivenGlobal()
        {
            yield break;
        }

        private IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            yield break;
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            yield break;
        }

        protected override IEnumerable<Event> Expect()
        {
            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);
            var events = new List<Event>
                {
                    new Events.Global.CompanyCreated(CompanyId, CompanyName,_timestamp11, systemAsPerson, _correlationId),

                    new EmployeeCreated(CompanyId, CompanyName, Constants.SystemUserId, new Login(Constants.SystemUserId, Constants.SystemUserId), string.Empty, string.Empty,
                        Constants.SystemUserId, _timestamp21, systemAsPerson, _correlationId),
                    new EmployeeCreated(CompanyId, CompanyName, AdminId, new Login(Constants.GoogleIdProvider, AdminEmail), AdminFirstName, string.Empty,
                        AdminLastName, _timestamp22, systemAsPerson, _correlationId),
                    new CompanyCreated(CompanyId, CompanyName, _timestamp31, systemAsPerson, _correlationId),
                    new CompanyAdminAdded(CompanyId, CompanyName, Constants.SystemUserId, NameService.GetName(string.Empty, Constants.SystemUserId), _timestamp32, systemAsPerson, _correlationId),
                    new EmployeeAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName, AdminLastName), new Login(Constants.GoogleIdProvider, AdminEmail), _timestamp33, systemAsPerson, _correlationId),
                    new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName, AdminLastName), _timestamp34, systemAsPerson, _correlationId)
                };
            return events;
        }
    }
}
