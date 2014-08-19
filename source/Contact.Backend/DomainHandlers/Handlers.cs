using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.UI;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Tasks;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;

namespace Contact.Backend.DomainHandlers
{
    public class Handlers
    {
        public static void CreateHandlers(IMediator mediator, IUnityContainer container)
        {
            // Add employee
            RegisterAddEmployee(mediator, container);
            RegisterTerminateEmployee(mediator, container);
            
            RegisterAddCompanyAdmin(mediator, container);
            RegisterRemoveCompanyAdmin(mediator, container);

            RegisterAddOfficeAdmin(mediator, container);
            RegisterRemoveOfficeAdmin(mediator, container);

            RegisterOpenOffice(mediator, container);
            RegisterCloseOffice(mediator, container);

        }

      
        private static void RegisterAddEmployee(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<AddEmployeeRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    Domain.ValueTypes.Address homeAddress = null;
                    Domain.ValueTypes.Picture photo = null;
                    if (req.HomeAddress != null)
                    {
                        homeAddress = new Domain.ValueTypes.Address(req.HomeAddress.Street,
                            req.HomeAddress.PostalCode, req.HomeAddress.PostalName);
                    }

                    if (req.Photo != null)
                    {
                        photo = new Domain.ValueTypes.Picture(req.Photo.Title, req.Photo.Extension,
                            req.Photo.Content);
                    }

                    //TODO: Evaluate TODO below
                    //TODO: Get version from readmodel

                    //TODO: Consider to expose competence-tags on web-api
                    CompetenceTag[] competence = null;

                    var identityResolver = container.Resolve<IResolveUserIdentity>();

                    //RV: 19.08.2014:
                    //TODO: Fix login here.
                    //new Login is not the same as the user requesting the command
                    
                    var command = new AddEmployee(req.CompanyId, req.OfficeId, req.GlobalId ?? CreateNewGlobalId(), new Login(GetProviderFromIdentity(user), req.Email, GetIdFromIdentity(user, identityResolver)), req.FirstName, req.LastName,
                        req.DateOfBirth, req.JobTitle, req.PhoneNumber, req.Email, homeAddress, photo, competence, DateTime.UtcNow, GetCreatedBy(user, identityResolver), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(container, command);
                    
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterTerminateEmployee(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<TerminateEmployeeRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new TerminateEmployee(req.CompanyId, req.OfficeId, req.EmployeeId,DateTime.UtcNow,GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()),correlationId,Domain.Constants.IgnoreVersion);

                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterAddCompanyAdmin(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<AddCompanyAdminRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new AddCompanyAdmin(req.CompanyId, req.NewAdminId, DateTime.UtcNow, GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()), correlationId, Domain.Constants.IgnoreVersion);
                    
                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterRemoveCompanyAdmin(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<RemoveCompanyAdminRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new RemoveCompanyAdmin(req.CompanyId, req.AdminId, DateTime.UtcNow, GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterAddOfficeAdmin(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<AddOfficeAdminRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new AddOfficeAdmin(req.CompanyId, req.OfficeId, req.AdminId, DateTime.UtcNow, GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }


        private static void RegisterRemoveOfficeAdmin(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<RemoveOfficeAdminRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new RemoveOfficeAdmin(req.CompanyId, req.OfficeId, req.AdminId, DateTime.UtcNow, GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterOpenOffice(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<OpenOfficeRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Include Address in request
                    Domain.ValueTypes.Address address = null;
                    //if (req.Address != null)
                    //{
                    //    Address = new Domain.ValueTypes.Address(req.Address.Street,
                    //        req.Address.PostalCode, req.Address.PostalName);
                    //}
                    //TODO: Get version from readmodel
                    var command = new OpenOffice(req.CompanyId, req.OfficeName, req.OfficeName, address, DateTime.UtcNow, GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterCloseOffice(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<CloseOfficeRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new CloseOffice(req.CompanyId, req.OfficeId, DateTime.UtcNow, GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static Person GetCreatedBy(IIdentity user, IResolveUserIdentity identityResolver)
        {
            return new Person(GetIdFromIdentity(user, identityResolver), user.GetUserName());
        }

        private static Response Send(IUnityContainer container, Command command)
        {
            var sender = container.Resolve<ICommandSender>();

            sender.Send(command);

            return Helpers.CreateResponse(command.CorrelationId);
        }

        private static string GetIdFromIdentity(IIdentity user, IResolveUserIdentity identityResolver)
        {
            var identity = user as ClaimsIdentity;
            if (identity != null)
            {
                var claims = identity;
                var id = claims.FindFirst(ClaimTypes.NameIdentifier);
                var globalIdentity = identityResolver.ResolveUserIdentityByProviderId(id.Issuer, id.Value);
                if (!string.IsNullOrEmpty(globalIdentity)) return globalIdentity;

                var email = claims.FindFirst(ClaimTypes.Email);
                return identityResolver.ResolveUserIdentityByEmail(email.Issuer, email.Value);
            }
            //return user.GetUserId();
            return string.Empty;
        }

        private static string CreateNewGlobalId()
        {
            return new Guid().ToString();
        }


        private static string GetProviderFromIdentity(IIdentity user)
        {
            var identity = user as ClaimsIdentity;
            if (identity != null)
            {
                var claims = identity;
                var id = claims.FindFirst(ClaimTypes.NameIdentifier);
                return id.Issuer;
            }
            
            return null;
        }

    }
}

