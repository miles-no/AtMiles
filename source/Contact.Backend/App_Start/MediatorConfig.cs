using System.Collections.Generic;
using AutoMapper;
using Contact.Backend.DomainHandlers;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Busy;
using Contact.Backend.Models.Api.Employee;
using Contact.Backend.Models.Api.Search;
using Contact.Backend.Models.Api.Status;
using Contact.Backend.Utilities;
using Contact.ReadStore.BusyTimeStore;
using Contact.ReadStore.SearchStore;
using Contact.ReadStore.SessionStore;
using Microsoft.Practices.Unity;

namespace Contact.Backend
{
    public class MediatorConfig
    {
        public static IMediator Create(IUnityContainer container)
        {
            var mediator = new Mediator();

            Handlers.CreateHandlers(mediator, container);

            
            mediator.Subscribe<SearchRequestModel, SearchResultModel>((s, user) =>
            {
                var engine = container.Resolve<EmployeeSearchEngine>();
                int total;
                var resSearch = engine.FulltextSearch(s.Query, s.Take, s.Skip, out total);

                var res = new SearchResultModel {Skipped = s.Skip, Total = total};
                res.Results = Mapper.Map<List<EmployeeSearchModel>,List<Result>>(resSearch);
                
                return res;

            });

            mediator.Subscribe<StatusRequest, StatusResponse>((sr, user) =>
            {
                var engine = container.Resolve<CommandStatusEngine>();
                var res = engine.GetStatus(sr.Id);
                return new StatusResponse
                {
                    Id = sr.Id,
                    Status = res.Status,
                    ErrorMessage = res.ErrorMessage,
                    Url = sr.SenderUrl
                };
            });

            mediator.Subscribe<EmployeeDetailsRequest, EmployeeDetailsResponse>((request, user) =>
            {
                var engine = container.Resolve<EmployeeSearchEngine>();
                var employee = engine.GetEmployeeSearchModel(request.EmployeeId);
                return Mapper.Map<EmployeeSearchModel, EmployeeDetailsResponse>(employee);
            });

            mediator.Subscribe<BusyTimeRequest, BusyTimeResponse>((request, user) =>
            {
                var employeeId = Helpers.GetUserIdentity(user);
                var engine = container.Resolve<BusyTimeEngine>();
                var data = engine.GetBusyTime(employeeId);
                return Mapper.Map<BusyTimeModel, BusyTimeResponse>(data);
            });
            return mediator;
        }
    }
}