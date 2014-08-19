using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoMapper;
using Contact.Backend.DomainHandlers;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Search;
using Contact.ReadStore.Test;
using Microsoft.Practices.Unity;
using NUnit.Framework;

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
                var engine = container.Resolve<SearchEngine>();
                int total;
                var resSearch = engine.FulltextSearch(s.Query, s.Take, s.Skip, out total);

                var res = new SearchResultModel {Skipped = s.Skip, Total = total};
                res.Results = Mapper.Map<List<PersonSearchModel>,List<Result>>(resSearch);
                
                return res;

            });

            return mediator;
        }
    }
}