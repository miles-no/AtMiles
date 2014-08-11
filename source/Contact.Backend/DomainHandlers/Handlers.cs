using System;
using System.Security.Principal;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api;
using Contact.Backend.Utilities;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
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
                    var command = new AddEmployee(req.CompanyId, req.OfficeId, req.GlobalId, req.FirstName, req.LastName,
                        req.DateOfBirth);

                    command.WithEmail(req.Email).WithJobTitle(req.JobTitle).WithPhoneNumber(req.PhoneNumber);

                    if (req.HomeAddress != null)
                    {
                        command.WithHomeAddress(new Domain.ValueTypes.Address(req.HomeAddress.Street,
                            req.HomeAddress.PostalCode, req.HomeAddress.PostalName));
                    }

                    if (req.Photo != null)
                    {
                        command.WithPhoto(new Domain.ValueTypes.Picture(req.Photo.Title, req.Photo.Extension,
                            req.Photo.Content));
                    }

                    command.WithCorrelationId(correlationId);

                    var sender = container.Resolve<ICommandSender>();

                    sender.Send(command);

                    return Helpers.CreateResponse(correlationId);
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
                    var command = new TerminateEmployee(req.CompanyId, req.OfficeId, req.EmployeeId);

                    return SetCorrelationAndSend(container, command, correlationId, user);
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
                    var command = new AddCompanyAdmin(req.CompanyId, req.NewAdminId);
                    
                    return SetCorrelationAndSend(container, command, correlationId, user);
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
                    var command = new RemoveCompanyAdmin(req.CompanyId, req.AdminId);

                    return SetCorrelationAndSend(container, command, correlationId, user);
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
                    var command = new AddOfficeAdmin(req.CompanyId, req.OfficeId, req.AdminId);

                    return SetCorrelationAndSend(container, command, correlationId, user);
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
                    var command = new RemoveOfficeAdmin(req.CompanyId, req.OfficeId,req.AdminId);

                    return SetCorrelationAndSend(container, command, correlationId, user);
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
                    var command = new OpenOffice(req.CompanyId, req.Name);

                    return SetCorrelationAndSend(container, command, correlationId, user);
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
                    var command = new CloseOffice(req.CompanyId, req.OfficeId);

                    return SetCorrelationAndSend(container, command, correlationId, user);
                }
                catch (Exception ex)
                {
                    return Helpers.CreateErrorResponse(correlationId, ex.Message);
                }
            });
        }

        private static Response SetCorrelationAndSend(IUnityContainer container, Command command, string correlationId,
            IIdentity user)
        {
            command.WithCorrelationId(correlationId);
            command.WithCreatedBy(new Person(user.GetUserId(), user.GetUserName()));

            var sender = container.Resolve<ICommandSender>();

            sender.Send(command);

            return Helpers.CreateResponse(correlationId);
        }
    }
}

