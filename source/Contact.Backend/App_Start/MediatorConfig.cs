using System.Collections.Generic;
using Contact.Backend.DomainHandlers;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Admins;
using Contact.Backend.Models.Api.Busy;
using Contact.Backend.Models.Api.Employee;
using Contact.Backend.Models.Api.Search;
using Contact.Backend.Models.Api.Status;
using Contact.Backend.Utilities;
using Contact.ReadStore.BusyTimeStore;
using Contact.ReadStore.SearchStore;
using Contact.ReadStore.SessionStore;
using Contact.ReadStore.UserStore;
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
                res.Results = Convert(resSearch);
                
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
                return Convert(employee);
            });

            mediator.Subscribe<BusyTimeRequest, BusyTimeResponse>((request, user) =>
            {
                var employeeId = Helpers.GetUserIdentity(user);
                var engine = container.Resolve<BusyTimeEngine>();
                var data = engine.GetBusyTime(employeeId);
                return Convert(data);
            });

            mediator.Subscribe<GetCompanyAdminsRequest, GetCompanyAdminsResponse>((request, user) =>
            {
                var engine = container.Resolve<UserLookupEngine>();
                var data = engine.GetAllCompanyAdmins(request.CompanyId);
                return Convert(data);
            });

            return mediator;
        }

        private static EmployeeDetailsResponse Convert(EmployeeSearchModel searchResult)
        {
            EmployeeDetailsResponse response = new EmployeeDetailsResponse();

            if (searchResult != null)
            {
                response.Id = searchResult.Id;
                response.GlobalId = searchResult.GlobalId;
                response.CompanyId = searchResult.CompanyId;
                response.OfficeName = searchResult.OfficeName;
                response.Name = searchResult.Name;
                response.DateOfBirth = searchResult.DateOfBirth;
                response.JobTitle = searchResult.JobTitle;
                response.PhoneNumber = searchResult.PhoneNumber;
                response.Email = searchResult.Email;
                response.PrivateAddress = Convert(searchResult.PrivateAddress);
                response.Thumb = searchResult.Thumb;
                response.Competency = Convert(searchResult.Competency);
                response.BusyTimeEntries = Convert(searchResult.BusyTimeEntries);
                response.KeyQualifications = searchResult.KeyQualifications;
                response.Descriptions = Convert(searchResult.Descriptions);
                response.Score = searchResult.Score;
            }


            return response;
        }

        private static List<EmployeeDetailsResponse.Description> Convert(IEnumerable<EmployeeSearchModel.Description> i)
        {
            var o = new List<EmployeeDetailsResponse.Description>();

            if (i != null)
            {
                foreach (var description in i)
                {
                    o.Add(new EmployeeDetailsResponse.Description {LocalDescription = description.LocalDescription, InternationalDescription = description.InternationalDescription});
                }
            }

            return o;
        }

        private static List<EmployeeDetailsResponse.BusyTime> Convert(IEnumerable<EmployeeSearchModel.BusyTime> i)
        {
            var o = new List<EmployeeDetailsResponse.BusyTime>();
            if (i != null)
            {
                foreach (var busyTime in i)
                {
                    o.Add(new EmployeeDetailsResponse.BusyTime
                    {
                        Id = busyTime.Id,
                        Start = busyTime.Start,
                        End = busyTime.End,
                        PercentageOccupied = busyTime.PercentageOccupied,
                        Comment = busyTime.Comment
                    });
                }
            }

            return o;
        }

        private static EmployeeDetailsResponse.Tag[] Convert(IEnumerable<Tag> i)
        {
            var o = new List<EmployeeDetailsResponse.Tag>();
            if (i != null)
            {
                foreach (var tag in i)
                {
                    o.Add(new EmployeeDetailsResponse.Tag {Category = tag.Category, Competency = tag.Competency, InternationalCompentency = tag.InternationalCompentency, InternationalCategory = tag.InternationalCategory});
                }
            }
            return o.ToArray();
        }

        private static EmployeeDetailsResponse.Address Convert(EmployeeSearchModel.Address i)
        {
            var o = new EmployeeDetailsResponse.Address();
            if (i != null)
            {
                o.Street = i.Street;
                o.PostalCode = i.PostalCode;
                o.PostalName = i.PostalName;
            }

            return o;
        }

        private static List<Result> Convert(IEnumerable<EmployeeSearchModel> busyTimeEnties)
        {
            var result = new List<Result>();
            if (busyTimeEnties != null)
            {
                foreach (var entry in busyTimeEnties)
                {
                    var convertedEntry = new Result
                    {
                        CompanyId = entry.CompanyId,
                        OfficeName = entry.OfficeName,
                        GlobalId = entry.GlobalId,
                        Name = entry.Name,
                        DateOfBirth = entry.DateOfBirth,
                        JobTitle = entry.JobTitle,
                        PhoneNumber = entry.PhoneNumber,
                        Email = entry.Email,
                        Thumb = entry.Thumb
                    };
                    if (entry.PrivateAddress != null)
                    {
                        convertedEntry.Address_Street = entry.PrivateAddress.Street;
                        convertedEntry.Address_PostalCode = entry.PrivateAddress.PostalCode;
                        convertedEntry.Address_PostalName = entry.PrivateAddress.PostalName;
                    }
                    result.Add(convertedEntry);
                }
            }
            return result;
        }

        private static GetCompanyAdminsResponse Convert(IEnumerable<UserLookupModel> busyTimeEnties)
        {
            var response = new GetCompanyAdminsResponse {Admins = new List<GetCompanyAdminsResponse.Admin>()};
            if (busyTimeEnties != null)
            {
                foreach (var userLookupModel in busyTimeEnties)
                {
                    response.Admins.Add(new GetCompanyAdminsResponse.Admin {Id = userLookupModel.GlobalId, Name = userLookupModel.Name});
                }
            }
            return response;
        }

        private static BusyTimeResponse Convert(BusyTimeModel data)
        {
            if (data == null) return null;
            var response = new BusyTimeResponse();
            response.ExpiryDate = data.ExpiryDate;
            response.BusyTimeEntries = new List<BusyTimeResponse.BusyTime>();
            if (data.BusyTimeEntries != null)
            {
                foreach (var busyTimeEntry in data.BusyTimeEntries)
                {
                    var time = Convert(busyTimeEntry);
                    if (time != null)
                    {
                        response.BusyTimeEntries.Add(time);
                    }
                }
            }
            return response;
        }

        private static BusyTimeResponse.BusyTime Convert(BusyTimeModel.BusyTime busyTimeEntry)
        {
            if (busyTimeEntry == null) return null;
            var time = new BusyTimeResponse.BusyTime
            {
                Id = busyTimeEntry.Id,
                Start = busyTimeEntry.Start,
                End = busyTimeEntry.End,
                PercentageOccupied = busyTimeEntry.PercentageOccupied,
                Comment = busyTimeEntry.Comment
            };

            return time;
        }
    }
}