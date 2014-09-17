using System.Threading.Tasks;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Events.Import;
using Contact.Domain.Services;
using Contact.Infrastructure;
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

        public static string GetRavenId(string id)
        {
            return "users/" + id;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            handler.RegisterHandler<EmployeeCreated>(HandleCreated);
            handler.RegisterHandler<EmployeeTerminated>(HandleTerminated);
            handler.RegisterHandler<CompanyAdminAdded>(HandleCompanyAdminAdded);
            handler.RegisterHandler<CompanyAdminRemoved>(HandleCompanyAdminRemoved);
            handler.RegisterHandler<ImportedFromCvPartner>(HandleImportCvPartner);
        }

        private async Task HandleImportCvPartner(ImportedFromCvPartner ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var existing = await session.LoadAsync<UserLookupModel>(GetRavenId(ev.EmployeeId));
                if (existing != null)
                {
                    existing = Patch(existing, ev);
                }
                else
                {
                    existing = Convert(ev);
                }

                await session.StoreAsync(existing);
                await session.SaveChangesAsync();
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

        private async Task HandleCompanyAdminRemoved(CompanyAdminRemoved ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var user = await session.LoadAsync<UserLookupModel>(GetRavenId(ev.AdminId));

                user = Patch(user, ev);
                await session.StoreAsync(user);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleCompanyAdminAdded(CompanyAdminAdded ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var user = await session.LoadAsync<UserLookupModel>(GetRavenId(ev.NewAdminId));

                user = Patch(user, ev);
                await session.StoreAsync(user);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleTerminated(EmployeeTerminated ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var user = await session.LoadAsync<UserLookupModel>(GetRavenId(ev.EmployeeId));
                if (user != null)
                {
                    session.Delete(user);
                    await session.SaveChangesAsync();
                }
            }
        }

        private async Task HandleCreated(EmployeeCreated ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var existing = await session.LoadAsync<UserLookupModel>(GetRavenId(ev.EmployeeId));
                if (existing != null)
                {
                    existing = Patch(existing, ev);
                }
                else
                {
                    existing = Convert(ev);
                }
                await session.StoreAsync(existing);
                await session.SaveChangesAsync();
            }
        }

        private static UserLookupModel Convert(EmployeeCreated ev)
        {
            return new UserLookupModel
            {
                Id = GetRavenId(ev.EmployeeId),
                GlobalProviderId = CreateGlobalId(ev),
                GlobalProviderEmail = CreateGlobalEmailId(ev),
                GlobalId = ev.EmployeeId,
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
    }
}