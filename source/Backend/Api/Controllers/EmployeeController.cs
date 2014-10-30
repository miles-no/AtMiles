using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using no.miles.at.Backend.Api.Models.Api.Busy;
using no.miles.at.Backend.Api.Models.Api.Employee;
using no.miles.at.Backend.Api.Models.Api.Tasks;
using no.miles.at.Backend.Api.Utilities;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.ReadStore.BusyTimeStore;
using no.miles.at.Backend.ReadStore.SearchStore;

namespace no.miles.at.Backend.Api.Controllers
{
    public class EmployeeController : ApiController
    {
        private readonly IResolveNameOfUser _nameResolver;
        private readonly ICommandSender _commandSender;
        private readonly BusyTimeEngine _busyTimeEngine;
        private readonly EmployeeSearchEngine _employeeEngine;

        public EmployeeController(IResolveNameOfUser nameResolver, ICommandSender commandSender, BusyTimeEngine busyTimeEngine, EmployeeSearchEngine employeeEngine)
        {
            _nameResolver = nameResolver;
            _commandSender = commandSender;
            _busyTimeEngine = busyTimeEngine;
            _employeeEngine = employeeEngine;
        }

        [HttpGet]
        [Route("api/company/{companyId}/employee/{employeeId}")]
        [ResponseType(typeof(EmployeeDetailsResponse))]
        [Authorize]
        public HttpResponseMessage GetEmployeeDetails(string employeeId)
        {
            var employee = _employeeEngine.GetEmployeeSearchModel(employeeId);
            var res = Convert(employee);

            if (Helpers.UserHasAccessToCompany(res.CompanyId) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "User does not have permission to view this employe");
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet]
        [Route("api/company/{companyId}/vcard/{employeeId}")]
        
        public HttpResponseMessage GetEmployeeVcard(string employeeId)
        {
            var employee = _employeeEngine.GetEmployeeSearchModel(employeeId);
            if (employee == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee is not found");
            }

            var vcard = GenerateVcardString(employee);

            var res = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(vcard, Encoding.UTF8, "text/vcard")
            };

            res.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("{0}.vcf",employee.Name)
            };

            return res;
        }

        private static string GenerateVcardString(EmployeeSearchModel employee)
        {
            var buffer = new StringBuilder();
            buffer.AppendLine("BEGIN:VCARD");
            buffer.AppendLine("VERSION:3.0");
            buffer.AppendFormat("N:{0};{1}\r\n", employee.LastName, employee.FirstName); // Last;First as specified in VCARD 3.0
            buffer.AppendFormat("FN:{0}\r\n", NameService.GetName(employee.FirstName, employee.MiddleName, employee.LastName)); // Last;First as specified in VCARD 3.0
            buffer.AppendFormat("EMAIL:{0}\r\n", employee.Email);
            buffer.AppendFormat("TEL;TYPE=cell:{0}\r\n", employee.PhoneNumber);
            buffer.AppendFormat("ORG:Miles;{0}\r\n", employee.OfficeName);
            buffer.AppendFormat("TITLE:{0}\r\n", employee.JobTitle);
            buffer.AppendFormat("BDAY:{0}\r\n", employee.DateOfBirth.ToString("yyyyMMdd"));
            buffer.AppendLine("URL:http://www.miles.no\r\n");

            if (employee.Thumb != null && employee.Thumb.Length > 100)
            {
                var filetype =
                    employee.Thumb.Substring(employee.Thumb.IndexOf("/"),
                        employee.Thumb.IndexOf(";") - employee.Thumb.IndexOf("/"))
                        .Replace("/", string.Empty)
                        .Replace(";", string.Empty)
                        .ToUpper();
                var image = employee.Thumb.Substring(employee.Thumb.IndexOf(",") + 1);

                buffer.AppendFormat("PHOTO;ENCODING=BASE64;TYPE={0}:{1}\r\n", filetype, image);
            }

            buffer.AppendLine("END:VCARD");
            return buffer.ToString();
        }

        [HttpGet]
        [Route("api/company/{companyId}/employee/busytime")]
        [ResponseType(typeof(BusyTimeResponse))]
        [Authorize]
        public BusyTimeResponse GetBusyTime(string companyId)
        {

            var employeeId = Helpers.GetUserIdentity(User.Identity);
            var data = _busyTimeEngine.GetBusyTime(employeeId);
            return Convert(data);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime")]
        [ResponseType(typeof(Response))]
        [Authorize]
        public HttpResponseMessage AddBusyTime(string companyId, DateTime start, DateTime? end, short percentageOccupied, string comment)
        {
            string correlationId = Helpers.CreateNewId();

            try
            {
                var createdBy = Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver);
                var command = new AddBusyTime(companyId, createdBy.Identifier, start, end, percentageOccupied, comment, DateTime.UtcNow, createdBy, correlationId, Constants.IgnoreVersion);
                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/company/{companyId}/employee/busytime/{busyTimeId}")]
        [ResponseType(typeof(Response))]
        [Authorize]
        public HttpResponseMessage RemoveBusyTime(string companyId, string busyTimeId)
        {
            string correlationId = Helpers.CreateNewId();
            try
            {
                var createdBy = Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver);
                var command = new RemoveBusyTime(companyId, createdBy.Identifier, busyTimeId, DateTime.UtcNow, createdBy, correlationId, Constants.IgnoreVersion);
                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime/{busyTimeId}")]
        [ResponseType(typeof(Response))]
        [Authorize]
        public HttpResponseMessage UpdateBusyTimeNewEnd(string companyId, string busyTimeId, DateTime start, DateTime? end, short percentageOccupied, string comment)
        {
            string correlationId = Helpers.CreateNewId();

            try
            {
                var createdBy = Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver);
                var command = new UpdateBusyTime(companyId, createdBy.Identifier, busyTimeId, start, end, percentageOccupied, comment, DateTime.UtcNow, createdBy, correlationId, Constants.IgnoreVersion);
                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime/confirm")]
        [ResponseType(typeof(Response))]
        [Authorize]
        public HttpResponseMessage ConfirmBusyTimeEntries(string companyId)
        {
            string correlationId = Helpers.CreateNewId();

            try
            {
                var createdBy = Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver);
                var command = new ConfirmBusyTimeEntries(companyId, createdBy.Identifier, DateTime.UtcNow, createdBy, correlationId, Constants.IgnoreVersion);
                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/setdateofbirth")]
        [ResponseType(typeof(Response))]
        [Authorize]
        public HttpResponseMessage SetDateOfBirth(string companyId, DateTime dateOfBirth)
        {
            string correlationId = Helpers.CreateNewId();

            try
            {
                var createdBy = Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver);
                var command = new SetDateOfBirth(companyId, createdBy.Identifier, dateOfBirth, DateTime.UtcNow, createdBy, correlationId, Constants.IgnoreVersion);
                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/setprivateaddress")]
        [ResponseType(typeof(Response))]
        [Authorize]
        public HttpResponseMessage SetPrivateAddress(string companyId, string street, string postalcode, string postalname)
        {
            string correlationId = Helpers.CreateNewId();

            try
            {
                var createdBy = Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver);
                var command = new SetPrivateAddress(companyId, createdBy.Identifier, new Address(street, postalcode, postalname), DateTime.UtcNow, createdBy, correlationId, Constants.IgnoreVersion);
                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
        }

        private static BusyTimeResponse Convert(BusyTimeModel data)
        {
            if (data == null) return null;
            var response = new BusyTimeResponse
            {
                ExpiryDate = data.ExpiryDate,
                BusyTimeEntries = new List<BusyTimeResponse.BusyTime>()
            };
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

        private static EmployeeDetailsResponse Convert(EmployeeSearchModel searchResult)
        {
            var response = new EmployeeDetailsResponse();
            if (searchResult != null)
            {
                response.Id = searchResult.Id;
                response.GlobalId = searchResult.GlobalId;
                response.CompanyId = searchResult.CompanyId;
                response.OfficeName = searchResult.OfficeName;
                response.Name = searchResult.Name;
                response.FirstName = searchResult.FirstName;
                response.LastName = searchResult.LastName;
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
                o.AddRange(i.Select(description => new EmployeeDetailsResponse.Description {LocalDescription = description.LocalDescription, InternationalDescription = description.InternationalDescription}));
            }

            return o;
        }

        private static List<EmployeeDetailsResponse.BusyTime> Convert(IEnumerable<EmployeeSearchModel.BusyTime> i)
        {
            var o = new List<EmployeeDetailsResponse.BusyTime>();
            if (i != null)
            {
                o.AddRange(i.Select(busyTime => new EmployeeDetailsResponse.BusyTime
                {
                    Id = busyTime.Id, Start = busyTime.Start, End = busyTime.End, PercentageOccupied = busyTime.PercentageOccupied, Comment = busyTime.Comment
                }));
            }

            return o;
        }

        private static EmployeeDetailsResponse.Tag[] Convert(IEnumerable<Tag> i)
        {
            var o = new List<EmployeeDetailsResponse.Tag>();
            if (i != null)
            {
                o.AddRange(i.Select(tag => new EmployeeDetailsResponse.Tag {Category = tag.Category, Competency = tag.Competency, InternationalCompentency = tag.InternationalCompentency, InternationalCategory = tag.InternationalCategory}));
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
    }
}
