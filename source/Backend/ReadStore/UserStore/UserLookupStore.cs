using System.Threading.Tasks;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Events.Import;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Infrastructure;
using Raven.Client;

namespace no.miles.at.Backend.ReadStore.UserStore
{
    public class UserLookupStore
    {
        private readonly IDocumentStore _documentStore;

        public UserLookupStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
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
                GlobalProviderId = ev.LoginId.Provider,
                GlobalProviderEmail = ev.LoginId.Email,
                GlobalId = ev.EmployeeId,
                Email = ev.LoginId.Email,
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
                GlobalProviderId = Constants.GoogleIdProvider,
                GlobalProviderEmail = ev.Email,
                GlobalId = ev.EmployeeId,
                Email = ev.Email,
                Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName),
                CompanyAdmin = false,
                CompanyId = ev.CompanyId,
            };
        }

// ReSharper disable once UnusedParameter.Local
        private static UserLookupModel Patch(UserLookupModel model, ImportedFromCvPartner ev)
        {
            return model;
        }

        private static UserLookupModel Patch(UserLookupModel model, EmployeeCreated ev)
        {
            model.Email = ev.LoginId.Email;
            model.Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName);
            return model;
        }

// ReSharper disable once UnusedParameter.Local
        private static UserLookupModel Patch(UserLookupModel model, CompanyAdminAdded ev)
        {
            model.CompanyAdmin = true;
            return model;
        }

// ReSharper disable once UnusedParameter.Local
        private static UserLookupModel Patch(UserLookupModel model, CompanyAdminRemoved ev)
        {
            model.CompanyAdmin = false;
            return model;
        }
    }
}