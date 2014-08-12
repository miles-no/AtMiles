﻿using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company.RemoveOfficeAdminTests
{
    [TestFixture]
    public class RemoveOfficeAdminAsCompanyAdmin : EventSpecification<RemoveOfficeAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeId = "SVG";
        private const string OfficeName = "Stavanger";

        private const string Admin1Id = "adm1";
        private const string Admin1FirstName = "Admin";
        private const string Admin1LastName = "Adminson";
        private static readonly DateTime Admin1DateOfBirth = new DateTime(1980, 01, 01);

        private const string Admin2Id = "adm2";
        private const string Admin2FirstName = "Adminsine";
        private const string Admin2LastName = "Adminsen";
        private static readonly DateTime Admin2DateOfBirth = new DateTime(1979, 01, 01);

        [Test]
        public void remove_office_admin_as_company_admin()
        {
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
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName)),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null)),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, OfficeId, OfficeName, Admin1Id, NameService.GetName(Admin1FirstName , Admin1LastName))),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, Admin1Id, NameService.GetName(Admin1FirstName , Admin1LastName))),
                    new FakeStreamEvent(CompanyId, new OfficeAdminAdded(CompanyId, CompanyName, OfficeId, OfficeName, Admin2Id, NameService.GetName(Admin2FirstName , Admin2LastName))),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(Admin1Id, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, Admin1Id, Admin1FirstName, Admin1LastName, Admin1DateOfBirth)),
                    new FakeStreamEvent(Admin2Id, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, Admin2Id, Admin2FirstName, Admin2LastName, Admin2DateOfBirth)),
                };
            return events;
        }

        public override RemoveOfficeAdmin When()
        {
            var cmd = new RemoveOfficeAdmin(CompanyId, OfficeId, Admin2Id)
                .WithCreated(DateTime.UtcNow)
                .WithCorrelationId(_correlationId)
                .WithBasedOnVersion(5)
                .WithCreatedBy(new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)));
            return (RemoveOfficeAdmin)cmd;
        }

        public override Handles<RemoveOfficeAdmin> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        public override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new OfficeAdminRemoved(CompanyId, CompanyName, OfficeId, OfficeName, Admin2Id, NameService.GetName(Admin2FirstName, Admin2LastName))
                                        .WithCorrelationId(_correlationId)
                                        .WithCreatedBy(new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)))
                };
            return events;
        }
    }
}