using AutoMapper;
using Contact.Backend.Models.Api.Employee;
using Contact.Backend.Models.Api.Search;
using Contact.ReadStore.SearchStore;
using Tag = Contact.ReadStore.SearchStore.Tag;

namespace Contact.Backend
{
    public static class MapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new SearchProfile());
                
            });
        }
    }

    public class SearchProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<EmployeeSearchModel, EmployeeDetailsResponse>();
            Mapper.CreateMap<EmployeeSearchModel.BusyTime, EmployeeDetailsResponse.BusyTime>();
            Mapper.CreateMap<EmployeeSearchModel.Description, EmployeeDetailsResponse.Description>();
        }
    }
}