using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Cv;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Employee;
using no.miles.at.Backend.Infrastructure;
using Convert = no.miles.at.Backend.Import.CvPartner.CvPartner.Converters.Convert;
using Image = no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Cv.Image;

namespace no.miles.at.Backend.Import.CvPartner.CvPartner
{
    public class ImportMiles : IImportDataFromCvPartner
    {
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
            var importData = new List<CvPartnerImportData>();

            var converter = new Convert("miles", null);
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"" + _accessToken + "\"";

            _logger.Debug("Download users from CvPartner");
            string employeesRaw = await client.DownloadStringTaskAsync("https://miles.cvpartner.no/api/v1/users");
            var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesRaw);

            _logger.Debug(string.Format("Done - {0} users", employees.Count));

            //TODO: Make this parallell using async

            foreach (var employee in employees)
            {
                var cv = await DownloadCv(employee, client);
                Picture employeePhoto = await DownloadPhoto(cv.Image, cv.Name);

                var importEmployee = converter.ToImportFromCvPartner(cv, employee, employeePhoto);
                importData.Add(importEmployee);

            }
            return importData;
        }

        private async Task<Cv> DownloadCv(Employee employee, WebClient client)
        {
            var url = "https://miles.cvpartner.no/api/v1/cvs/" + employee.UserId + "/" + employee.DefaultCvId;
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
