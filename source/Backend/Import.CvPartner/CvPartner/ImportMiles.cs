using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Cv;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Employee;
using no.miles.at.Backend.Infrastructure;
using Convert = no.miles.at.Backend.Import.CvPartner.CvPartner.Converters.Converter;

namespace no.miles.at.Backend.Import.CvPartner.CvPartner
{
    public class ImportMiles : IImportDataFromCvPartner
    {
        private const string UsersUrl = "https://miles.cvpartner.no/api/v1/users";
        private const string CvBaseUrl = "https://miles.cvpartner.no/api/v1/cvs/";
        private readonly string _accessToken;
        private readonly ILog _logger;

        public ImportMiles(string accessToken, ILog logger)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token must be set!");
            }
            _accessToken = accessToken;

            _logger = logger;
        }

        public async Task<List<CvPartnerImportData>> GetImportData()
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"" + _accessToken + "\"";

            _logger.Debug("Download users from CvPartner");
            string employeesRaw = await client.DownloadStringTaskAsync(UsersUrl);
            var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesRaw);

            _logger.Debug(string.Format("Done - {0} users", employees.Count));

            var promises = employees.Select(DownloadAdditionalData).ToList();

            await Task.WhenAll(promises);
            return promises.Select(promise => promise.Result).ToList();
        }

        private async Task<CvPartnerImportData> DownloadAdditionalData(Employee employee)
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"" + _accessToken + "\"";
            var cv = await DownloadCv(employee, client);
            var employeePhoto = await DownloadService.DownloadPhoto(cv.Image != null ? cv.Image.Url: null, cv.Name, _logger.Debug, _logger.Error);

            var importEmployee = Convert.ToImportFromCvPartner(cv, employee, employeePhoto);
            return importEmployee;
        }

        private async Task<Cv> DownloadCv(Employee employee, WebClient client)
        {
            var url = CvBaseUrl + employee.UserId + "/" + employee.DefaultCvId;
            _logger.Debug(string.Format("Downloading CV for {0} on url {1}", employee.Name, url));
            string rawCv = await client.DownloadStringTaskAsync(url);
            var cv = JsonConvert.DeserializeObject<Cv>(rawCv);
            return cv;
        }
       
    }
}
