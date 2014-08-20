﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoMapper;
using Contact.Backend.DomainHandlers;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Search;
using Contact.Backend.Models.Api.Status;
using Contact.ReadStore.Test;
using Contact.ReadStore.Test.SearchStore;
using Contact.ReadStore.Test.SessionStore;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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

            return mediator;
        }
    }
}