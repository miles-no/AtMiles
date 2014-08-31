using System.Linq;
using AutoMapper;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Events.Import;
using Contact.Domain.Services;
using Contact.Infrastructure;
using Raven.Abstractions.Data;
using Raven.Client;

namespace Contact.ReadStore.UserStore
{
    public class UserLookupStore
    {
        private readonly UserLookupEngine engine;
        private readonly IDocumentStore _documentStore;

        public UserLookupStore(UserLookupEngine engine, IDocumentStore documentStore)
        {
            this.engine = engine;
            this._documentStore = documentStore;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            Mapper.CreateMap<EmployeeCreated, User>()
                .ForMember(dest => dest.Name, source => source.MapFrom(e => NameService.GetName(e.FirstName, e.MiddleName, e.LastName)))
                .ForMember(dest => dest.LoginId, source => source.MapFrom(s => CreateGlobalId(s)));

            Mapper.CreateMap<ImportedFromCvPartner, User>()
                .ForMember(dest => dest.Name,
                    source => source.MapFrom(e => NameService.GetName(e.FirstName, e.MiddleName, e.LastName)));
           
            handler.RegisterHandler<EmployeeCreated>(HandleCreated);
            handler.RegisterHandler<EmployeeTerminated>(HandleTerminated);
            handler.RegisterHandler<CompanyAdminAdded>(HandleCompanyAdminAdded);
            handler.RegisterHandler<CompanyAdminRemoved>(HandleCompanyAdminRemoved);
            handler.RegisterHandler<OfficeAdminAdded>(HandleOfficeAdminAdded);
            handler.RegisterHandler<OfficeAdminRemoved>(HandleOfficeAdminRemoved);
            handler.RegisterHandler<OfficeClosed>(HandleOfficeClosed);
            handler.RegisterHandler<ImportedFromCvPartner>(HandleImportCvPartner);
        }
        
        private void HandleImportCvPartner(ImportedFromCvPartner ev)
        {
            using (var session = _documentStore.OpenSession())
            {

                var existing =
                   session.Query<User, UserLookupIndex>().FirstOrDefault(w => w.GlobalId == ev.EmployeeId);
                if (existing != null)
                {
                    Mapper.Map(ev, existing);
                }
                else
                {
                    existing = Mapper.Map<ImportedFromCvPartner, User>(ev);
                }

                session.Store(existing);
                session.SaveChanges();
            }
        }

        private static string CreateGlobalId(EmployeeCreated ev)
        {
            if (ev.LoginId == null) return null;
            
            return IdService.IdsToSingleLoginId(ev.CompanyId, ev.LoginId.Provider, ev.LoginId.Id);
        }

        private void HandleOfficeClosed(OfficeClosed ev)
        {

            _documentStore.DatabaseCommands.UpdateByIndex(typeof (UserLookupIndex).Name,
                new IndexQuery {Query = "CompanyId:" + ev.CompanyId},
                new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Remove,
                        Name = "AdminForOffices",
                        Value = ev.OfficeId
                    }
                }, false);


        }

        private void HandleOfficeAdminRemoved(OfficeAdminRemoved ev)
        {
            _documentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
                 new IndexQuery { Query = "GlobalId:" + ev.AdminId },
                 new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Remove,
                        Name = "AdminForOffices",
                        Value = ev.OfficeId
                    }
                }, false);
        }

        private void HandleOfficeAdminAdded(OfficeAdminAdded ev)
        {
            _documentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
                 new IndexQuery { Query = "GlobalId:" + ev.AdminId },
                 new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Add,
                        Name = "AdminForOffices",
                        Value = ev.OfficeId
                    }
                }, false);
        }

        private void HandleCompanyAdminRemoved(CompanyAdminRemoved ev)
        {
            _documentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
                new IndexQuery { Query = "GlobalId:" + ev.AdminId },
                new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Set,
                        Name = "CompanyAdmin",
                        Value = false
                    }
                }, false);
        }

        private void HandleCompanyAdminAdded(CompanyAdminAdded ev)
        {
            _documentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
               new IndexQuery { Query = "GlobalId:" + ev.NewAdminId },
               new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Set,
                        Name = "CompanyAdmin",
                        Value = true
                    }
                }, false);
        }

        private void HandleTerminated(EmployeeTerminated ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var user =
                    session.Query<User, UserLookupIndex>().FirstOrDefault(w => w.GlobalId == ev.EmployeeId);
                if (user != null)
                {
                    session.Delete(user);
                }
                
                session.SaveChanges();
            }
        }
      
        private void HandleCreated(EmployeeCreated ev)
        {

            using (var session = _documentStore.OpenSession())
            {
                var existing =
                    session.Query<User, UserLookupIndex>().FirstOrDefault(w => w.GlobalId == ev.EmployeeId);
                if (existing != null)
                {
                    Mapper.Map(ev, existing);
                }
                else
                {
                    existing = Mapper.Map<EmployeeCreated, User>(ev);
                }
                session.Store(existing);
                session.SaveChanges();
            }
        }
    }
}