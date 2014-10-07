using System;
using System.Net.Http;
using System.Security.Principal;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Tasks;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;
using Microsoft.Practices.Unity;

namespace Contact.Backend.DomainHandlers
{
    public class Handlers
    {
        public static void CreateHandlers(IMediator mediator, IUnityContainer container)
        {
            RegisterAddCompanyAdmin(mediator, container);
            RegisterRemoveCompanyAdmin(mediator, container);

            RegisterImportFromCvPartner(mediator, container);

            RegisterAddBusyTime(mediator, container);
            RegisterRemoveBusyTime(mediator, container);
            RegisterConfirmBusyTimeEntries(mediator, container);

            RegisterUpdateBusyTimeSetEnd(mediator, container);
            RegisterUpdateBusyTimeChangePercentage(mediator, container);

            RegisterSetDateOfBirth(mediator, container);
            RegisterSetPrivateAddress(mediator, container);

        }

        private static void RegisterSetPrivateAddress(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<SetPrivateAddressRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var nameResolver = container.Resolve<IResolveNameOfUser>();
                    var createdBy = GetCreatedBy(req.CompanyId, user, nameResolver);
                    var command = new SetPrivateAddress(req.CompanyId, createdBy.Identifier, new Address(req.Street, req.PostalCode, req.PostalName), DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterSetDateOfBirth(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<SetDateOfBirthRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var createdBy = GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>());
                    var command = new SetDateOfBirth(req.CompanyId, createdBy.Identifier, req.DateOfBirth, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterUpdateBusyTimeChangePercentage(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<UpdateBusyTimeChangePercentageRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var createdBy = GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>());
                    var command = new UpdateBusyTimeChangePercentage(req.CompanyId, createdBy.Identifier, req.BustTimeEntryId, req.NewPercentageOccupied, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterUpdateBusyTimeSetEnd(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<UpdateBusyTimeSetEndRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var createdBy = GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>());
                    var command = new UpdateBusyTimeSetEndDate(req.CompanyId, createdBy.Identifier, req.BustTimeEntryId, req.NewEnd, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterAddBusyTime(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<AddBusyTimeRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var createdBy = GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>());
                    var command = new AddBusyTime(req.CompanyId, createdBy.Identifier, req.Start, req.End, req.PercentageOccupied, req.Comment, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterRemoveBusyTime(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<RemoveBusyTimeRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var createdBy = GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>());
                    var command = new RemoveBusyTime(req.CompanyId, createdBy.Identifier, req.BustTimeEntryId, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterConfirmBusyTimeEntries(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<ConfirmBusyTimeEntriesRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var createdBy = GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>());
                    var command = new ConfirmBusyTimeEntries(req.CompanyId, createdBy.Identifier, DateTime.UtcNow, createdBy, correlationId, Domain.Constants.IgnoreVersion);
                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterImportFromCvPartner(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<ImportFromCvPartnerRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    var command = new ImportDataFromCvPartner(req.CompanyId, DateTime.UtcNow, GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(req.Request, container, command);

                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterAddCompanyAdmin(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<AddCompanyAdminRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new AddCompanyAdmin(req.CompanyId, req.NewAdminId, DateTime.UtcNow, GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static void RegisterRemoveCompanyAdmin(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<RemoveCompanyAdminRequest, HttpResponseMessage>((req, user) =>
            {
                string correlationId = Helpers.CreateNewId();

                try
                {
                    //TODO: Get version from readmodel
                    var command = new RemoveCompanyAdmin(req.CompanyId, req.AdminId, DateTime.UtcNow, GetCreatedBy(req.CompanyId, user, container.Resolve<IResolveNameOfUser>()), correlationId, Domain.Constants.IgnoreVersion);

                    return Send(req.Request, container, command);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(req.Request, correlationId, ex.Message);
                }
            });
        }

        private static Person GetCreatedBy(string companyId, IIdentity user, IResolveNameOfUser nameResolver)
        {
            var userId = Helpers.GetUserIdentity(user);
            var name = nameResolver.ResolveUserNameById(companyId, userId);
            return new Person(userId, name);
        }

        private static HttpResponseMessage Send(HttpRequestMessage request, IUnityContainer container, Command command)
        {
            var sender = container.Resolve<ICommandSender>();

            sender.Send(command);
            var response = Helpers.CreateResponse(request, command.CorrelationId);
            return response;
        }
    }
}