using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Cv;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Employee;
using no.miles.at.Backend.Infrastructure;
using Convert = no.miles.at.Backend.Import.CvPartner.CvPartner.Converters.Converter;
using Image = no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Cv.Image;

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
            //TODO: Test this code
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"" + _accessToken + "\"";

            _logger.Debug("Download users from CvPartner");
            string employeesRaw = await client.DownloadStringTaskAsync(UsersUrl);
            var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesRaw);

            _logger.Debug(string.Format("Done - {0} users", employees.Count));

            var promises = employees.Select(employee => DownloadAdditionalData(employee, client)).ToList();

            await Task.WhenAll(promises);
            return promises.Select(promise => promise.Result).ToList();
        }

        private async Task<CvPartnerImportData> DownloadAdditionalData(Employee employee, WebClient client)
        {
            var cv = await DownloadCv(employee, client);
            var employeePhoto = await DownloadPhoto(cv.Image, cv.Name);

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

        private async Task<Picture> DownloadPhoto(Image image, string name)
        {
            Picture photo = null;
            if (image != null && image.Url != null)
            {
                byte[] picture = null;
                string extension = null;
                string contentType = string.Empty;

                try
                {
                    var client = new WebClient();
                    picture = await client.DownloadDataTaskAsync(image.Url);
                    contentType = client.ResponseHeaders["Content-Type"];

                    var urlWithoutQueryParameters = image.Url.Substring(0, image.Url.IndexOf("?", StringComparison.Ordinal));
                    extension = urlWithoutQueryParameters.Substring(image.Url.LastIndexOf(".", StringComparison.Ordinal))
                        .Replace(".", string.Empty);
                    _logger.Debug(string.Format("Found image of . {0} format", extension));
                }
                catch (Exception ex)
                {
                    _logger.Error("Error downloading image", ex);
                }
                if (picture != null)
                {
                    var hash = MD5.Create().ComputeHash(picture);
                    photo = new Picture(name, extension, picture, contentType, hash);
                }
            }
            return photo;
        }
    }
}
