using System;
using System.Collections.Generic;
using System.Linq;
using Contact.Domain;
using Contact.Domain.Commands;
using Raven.Abstractions.Linq;
using Raven.Client;
using Raven.Client.Embedded;

namespace Contact.Backend.MockStore
{
    public class Event
    {
        public string Id { get; set; }
        public string CommandName { get; set; }
        public DynamicJsonObject Command { get; set; }
        public string BelongsTo { get; set; }
    }

    public class Company
    {
        public string Id { get; set; }
        public List<string> Offices { get; set; }
        public List<string> Admins { get; set; }
    }

    public class Translation
    {
        public string Language { get; set; }
        public string Value { get; set; }
    }

    public class Language
    {
        public string Id { get; set; }
        public Translation LanguageName { get; set; }
    }

    public class Office
    {
        public string Id { get; set; }
        public List<string> Admins { get; set; }
        public string Name { get; set; }
    }

    public class Employee
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OfficeId { get; set; }
    }

    public class User
    {
        public string Id { get; set; }

        
    }

    public class MockStore :
        Handles<AddCompanyAdmin>,
        Handles<RemoveCompanyAdmin>,
        Handles<RemoveOfficeAdmin>,
        Handles<AddOfficeAdmin>,
        Handles<OpenOffice>,
        Handles<CloseOffice>,
        Handles<AddEmployee>,
        Handles<TerminateEmployee>
    {
        public static IDocumentStore DocumentStore;

        static MockStore()
        {
            DocumentStore = new EmbeddableDocumentStore();
            DocumentStore.Initialize();
        }
        

        public void Handle(AddCompanyAdmin message)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var company = session.Query<Company>().FirstOrDefault(c => c.Id == message.CompanyId);
                if (company == null)
                {
                    company = new Company();
                    company.Admins = new List<string> {message.NewAdminId};
                    company.Offices = new List<string>();
                    
                    session.Store(company);
                    session.Store(new Event{BelongsTo = company.Id, CommandName = message.GetType().Name, Command =null});
                    //TODO::all the rest
                }

            }
        }

        public void Handle(RemoveCompanyAdmin message)
        {
            throw new NotImplementedException();
        }

        public void Handle(RemoveOfficeAdmin message)
        {
            throw new NotImplementedException();
        }

        public void Handle(AddOfficeAdmin message)
        {
            throw new NotImplementedException();
        }

        public void Handle(OpenOffice message)
        {
            throw new NotImplementedException();
        }

        public void Handle(CloseOffice message)
        {
            throw new NotImplementedException();
        }

        public void Handle(AddEmployee message)
        {
            throw new NotImplementedException();
            
        }

        public void Handle(TerminateEmployee message)
        {
            throw new NotImplementedException();
        }
    }
}
