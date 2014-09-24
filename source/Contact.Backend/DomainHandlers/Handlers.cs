using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Tasks;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;
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

            RegisterImportFromCvPartner(mediator, container);

            RegisterAddBusyTime(mediator, container);
            RegisterRemoveBusyTime(mediator, container);
            RegisterConfirmBusyTimeEntries(mediator, container);

            RegisterUpdateBusyTimeSetEnd(mediator, container);
            RegisterUpdateBusyTimeChangePercentage(mediator, container);

        }

        private static void RegisterUpdateBusyTimeChangePercentage(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<UpdateBusyTimeChangePercentageRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var identityResolver = container.Resolve<IResolveUserIdentity>();
                    var createdBy = GetCreatedBy(user, identityResolver);
                    var command = new UpdateBusyTimeChangePercentage(req.CompanyId, createdBy.Identifier, req.BustTimeEntryId, req.NewPercentageOccupied, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterUpdateBusyTimeSetEnd(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<UpdateBusyTimeSetEndRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var identityResolver = container.Resolve<IResolveUserIdentity>();
                    var createdBy = GetCreatedBy(user, identityResolver);
                    var command = new UpdateBusyTimeSetEndDate(req.CompanyId, createdBy.Identifier, req.BustTimeEntryId, req.NewEnd, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterAddBusyTime(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<AddBusyTimeRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var identityResolver = container.Resolve<IResolveUserIdentity>();
                    var createdBy = GetCreatedBy(user, identityResolver);
                    var command = new AddBusyTime(req.CompanyId, createdBy.Identifier, req.Start, req.End, req.PercentageOccupied, req.Comment, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterRemoveBusyTime(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<RemoveBusyTimeRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var identityResolver = container.Resolve<IResolveUserIdentity>();
                    var createdBy = GetCreatedBy(user, identityResolver);
                    var command = new RemoveBusyTime(req.CompanyId, createdBy.Identifier, req.BustTimeEntryId, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterConfirmBusyTimeEntries(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<ConfirmBusyTimeEntriesRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var identityResolver = container.Resolve<IResolveUserIdentity>();
                    var createdBy = GetCreatedBy(user, identityResolver);
                    var command = new ConfirmBusyTimeEntries(req.CompanyId, createdBy.Identifier, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static void RegisterImportFromCvPartner(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<ImportFromCvPartnerRequest, Response>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var identityResolver = container.Resolve<IResolveUserIdentity>();

                    var command = new ImportDataFromCvPartner(req.CompanyId, DateTime.UtcNow, GetCreatedBy(user, identityResolver), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(container, command);

                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
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
                        var hash = MD5.Create().ComputeHash(req.Photo.Content);
                        photo = new Domain.ValueTypes.Picture(req.Photo.Title, req.Photo.Extension,
                            req.Photo.Content, req.Photo.ContentType, hash);
                    }

                    //TODO: Evaluate TODO below
                    //TODO: Get version from readmodel


                    var identityResolver = container.Resolve<IResolveUserIdentity>();

                    //RV: 19.08.2014:
                    //TODO: Fix login here.
                    //new Login is not the same as the user requesting the command

                    var command = new AddEmployee(req.CompanyId, req.GlobalId ?? CreateNewGlobalId(), new Login(GetProviderFromIdentity(user), req.Email, Helpers.GetUserIdentity(user, identityResolver)), req.FirstName, req.LastName,
                        req.DateOfBirth, req.JobTitle, req.OfficeName, req.PhoneNumber, req.Email, homeAddress, photo, DateTime.UtcNow, GetCreatedBy(user, identityResolver), correlationId, Domain.Constants.IgnoreVersion);

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
                    var command = new TerminateEmployee(req.CompanyId, req.EmployeeId, DateTime.UtcNow, GetCreatedBy(user, container.Resolve<IResolveUserIdentity>()), correlationId, Domain.Constants.IgnoreVersion);

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

        private static Person GetCreatedBy(IIdentity user, IResolveUserIdentity identityResolver)
        {
            return new Person(Helpers.GetUserIdentity(user, identityResolver), user.GetUserName());
        }

        private static Response Send(IUnityContainer container, Command command)
        {
            var sender = container.Resolve<ICommandSender>();

            sender.Send(command);

            return Helpers.CreateResponse(command.CorrelationId);
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