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

        private static string GetRavenId(string id)
        {
            return "users/" + id;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
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
                var existing = session.Load<UserLookupModel>(GetRavenId(ev.EmployeeId));
                if (existing != null)
                {
                    existing = Patch(existing, ev);
                }
                else
                {
                    existing = Convert(ev);
                }

                session.Store(existing);
                session.SaveChanges();
            }
        }

        private static string CreateGlobalId(EmployeeCreated ev)
        {
            if (ev.LoginId == null) return string.Empty;
            if (string.IsNullOrEmpty(ev.LoginId.Id)) return string.Empty;

            return IdService.IdsToSingleLoginId(ev.CompanyId, ev.LoginId.Provider, ev.LoginId.Id);
        }

        private static string CreateGlobalEmailId(EmployeeCreated ev)
        {
            if (ev.LoginId == null) return null;
            if (string.IsNullOrEmpty(ev.LoginId.Email)) return string.Empty;

            return IdService.IdsToSingleEmailId(ev.CompanyId, ev.LoginId.Provider, ev.LoginId.Email);
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
            using (var session = _documentStore.OpenSession())
            {
                var user = session.Load<UserLookupModel>(GetRavenId(ev.AdminId));
                user = Patch(user, ev);
                session.Store(user);
                session.SaveChanges();
            }
        }

        private void HandleOfficeAdminAdded(OfficeAdminAdded ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var user = session.Load<UserLookupModel>(GetRavenId(ev.AdminId));
                user = Patch(user, ev);
                session.Store(user);
                session.SaveChanges();
            }
        }

        private void HandleCompanyAdminRemoved(CompanyAdminRemoved ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var user = session.Load<UserLookupModel>(GetRavenId(ev.AdminId));

                user = Patch(user, ev);
                session.Store(user);
                session.SaveChanges();
            }
        }

        private void HandleCompanyAdminAdded(CompanyAdminAdded ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var user = session.Load<UserLookupModel>(GetRavenId(ev.NewAdminId));

                user = Patch(user, ev);
                session.Store(user);
                session.SaveChanges();
            }
        }

        private void HandleTerminated(EmployeeTerminated ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var user = session.Load<UserLookupModel>(GetRavenId(ev.EmployeeId));
                if (user != null)
                {
                    session.Delete(user);
                    session.SaveChanges();
                }
            }
        }
      
        private void HandleCreated(EmployeeCreated ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var existing = session.Load<UserLookupModel>(GetRavenId(ev.EmployeeId));
                if (existing != null)
                {
                    existing = Patch(existing, ev);
                }
                else
                {
                    existing = Convert(ev);
                }
                session.Store(existing);
                session.SaveChanges();
            }
        }

        private static UserLookupModel Convert(EmployeeCreated ev)
        {
            return new UserLookupModel
            {
                Id = GetRavenId(ev.EmployeeId),
                GlobalProviderId = CreateGlobalId(ev),
                GlobalProviderEmail = CreateGlobalEmailId(ev),
                Email = ev.Email,
                Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName),
                CompanyAdmin = false,
                CompanyId = ev.CompanyId,
            };
        }

        private static UserLookupModel Convert(ImportedFromCvPartner ev)
        {
            return new UserLookupModel
            {
                Id = GetRavenId(ev.EmployeeId),
                //GlobalProviderEmail = CreateGlobalEmailId(ev),
                Email = ev.Email,
                Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName),
                CompanyAdmin = false,
                CompanyId = ev.CompanyId,
            };
        }

        private static UserLookupModel Patch(UserLookupModel model, ImportedFromCvPartner ev)
        {
            return model;
        }

        private static UserLookupModel Patch(UserLookupModel model, EmployeeCreated ev)
        {
            model.Email = ev.Email;
            model.Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName);
            return model;
        }

        private static UserLookupModel Patch(UserLookupModel model, CompanyAdminAdded ev)
        {
            model.CompanyAdmin = true;
            return model;
        }

        private static UserLookupModel Patch(UserLookupModel model, CompanyAdminRemoved ev)
        {
            model.CompanyAdmin = true;
            return model;
        }

        private static UserLookupModel Patch(UserLookupModel model, OfficeAdminAdded ev)
        {
            model.AdminForOffices.Add(ev.OfficeId);
            return model;
        }

        private static UserLookupModel Patch(UserLookupModel model, OfficeAdminRemoved ev)
        {
            model.AdminForOffices.RemoveAll(o => o == ev.OfficeId);
            return model;
        }
        
    }
}