using System;
using System.Web;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models;
using Contact.Backend.Models.Api.AdminModels;
using Contact.Backend.Models.Api.StatusModels;
using Contact.Domain;
using Contact.Domain.Commands;
using Microsoft.Practices.Unity;

namespace Contact.Backend.DomainHandlers
{
    public static class Handlers
    {
        static Handlers()
        {
            AutoMapper.Mapper.CreateMap<Employee, AddEmployee>();

        }

        
        public static void CreateHandlers(IMediator mediator, IUnityContainer container)
        {
            mediator.Subscribe<AddEmployeeRequest, Status>((req, user) =>
            {
                var command = AutoMapper.Mapper.Map<AddEmployeeRequest, AddEmployee>(req);

                string correlationId = Guid.NewGuid().ToString();

                container.Resolve <Handles<AddEmployee>>();
                //evt 
             //   rabbitClient.Send(command);
                
                
                return new Status{Id = correlationId, Url = "blabla"};

            });
        }
    }
}