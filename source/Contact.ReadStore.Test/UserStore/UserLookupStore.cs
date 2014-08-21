using System.Linq;
using AutoMapper;
using Contact.Backend.MockStore;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Services;
using Contact.Infrastructure;
using Raven.Abstractions.Data;

namespace Contact.ReadStore.Test.UserStore
{
    public class UserLookupStore
    {
        public void PrepareHandler(ReadModelHandler handler)
        {
            Mapper.CreateMap<EmployeeCreated, User>()
                .ForMember(dest => dest.Name, source => source.MapFrom(m => CreateName(m)))
                .ForMember(dest => dest.GlobalId, source => source.MapFrom(s => CreateGlobalId(s)));
                

            handler.RegisterHandler<EmployeeCreated>(HandleCreated);
            handler.RegisterHandler<EmployeeTerminated>(HandleTerminated);
            handler.RegisterHandler<CompanyAdminAdded>(HandleCompanyAdminAdded);
            handler.RegisterHandler<CompanyAdminRemoved>(HandleCompanyAdminRemoved);
            handler.RegisterHandler<OfficeAdminAdded>(HandleOfficeAdminAdded);
            handler.RegisterHandler<OfficeAdminRemoved>(HandleOfficeAdminRemoved);
            handler.RegisterHandler<OfficeClosed>(HandleOfficeClosed);

           
        }

        private static string CreateGlobalId(EmployeeCreated employee)
        {
            if (employee.LoginId != null)
            {
                return IdService.IdsToSingle(employee.CompanyId, employee.LoginId.Provider, employee.LoginId.Id);
            }
            return null;
        }

        private void HandleOfficeClosed(OfficeClosed office)
        {

            MockStore.DocumentStore.DatabaseCommands.UpdateByIndex(typeof (UserLookupIndex).Name,
                new IndexQuery {Query = "CompanyId:" + office.CompanyId},
                new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Remove,
                        Name = "AdminForOffices",
                        Value = office.OfficeId
                    }
                }, false);


        }

        private void HandleOfficeAdminRemoved(OfficeAdminRemoved officeAdmin)
        {
            MockStore.DocumentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
                 new IndexQuery { Query = "GlobalId:" + officeAdmin.AdminId },
                 new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Remove,
                        Name = "AdminForOffices",
                        Value = officeAdmin.OfficeId
                    }
                }, false);
        }

        private void HandleOfficeAdminAdded(OfficeAdminAdded officeAdmin)
        {
            MockStore.DocumentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
                 new IndexQuery { Query = "GlobalId:" + officeAdmin.AdminId },
                 new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Add,
                        Name = "AdminForOffices",
                        Value = officeAdmin.OfficeId
                    }
                }, false);
        }

        private void HandleCompanyAdminRemoved(CompanyAdminRemoved companyAdmin)
        {
            MockStore.DocumentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
                new IndexQuery { Query = "GlobalId:" + companyAdmin.AdminId },
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

        private void HandleCompanyAdminAdded(CompanyAdminAdded companyAdmin)
        {
            MockStore.DocumentStore.DatabaseCommands.UpdateByIndex(typeof(UserLookupIndex).Name,
               new IndexQuery { Query = "GlobalId:" + companyAdmin.NewAdminId },
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

        private void HandleTerminated(EmployeeTerminated employee)
        {
            using (var session = MockStore.DocumentStore.OpenSession())
            {
                var user =
                    session.Query<User, UserLookupIndex>().FirstOrDefault(w => w.GlobalId == employee.Id);
                if (user != null)
                {
                    session.Delete(user);
                }
                
                session.SaveChanges();
            }
        }

  
      
        private static void HandleCreated(EmployeeCreated employee)
        {

            var searchModel = Mapper.Map<EmployeeCreated, User>(employee);
            using (var session = MockStore.DocumentStore.OpenSession())
            {
                session.Store(searchModel);
                session.SaveChanges();
            }
        }

        private static string CreateName(EmployeeCreated employee)
        {
            return employee.FirstName + " " + (string.IsNullOrEmpty(employee.MiddleName) ? string.Empty : (employee.MiddleName + " ")) + employee.LastName;
        }

    }
}