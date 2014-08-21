using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.Events.Import;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner.Models.Cv;
using Contact.Import.CvPartner.CvPartner.Models.Employee;
using Contact.Infrastructure;
using Newtonsoft.Json;
using Image = Contact.Import.CvPartner.CvPartner.Models.Cv.Image;

namespace Contact.Import.CvPartner.CvPartner
{
    public class ImportMiles
    {

        public List<AddEmployee> AddEmployeesCommands { get; set; }
        public List<OpenOffice> OpenOfficeCommands { get; set; }
        public List<AddCompanyAdmin> AddCompanyAdminsCommands { get; set; }


        public async Task<List<ImportFromCvPartner>> GetImportData(string accessToken, Person createdBy, string correlationId)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token must be set!");
            }

            var importData = new List<ImportFromCvPartner>();

            var converter = new Converters.Convert("miles", createdBy, Log);
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"" + accessToken + "\"";

            Log("Download users from CvPartner...");
            string employeesRaw = client.DownloadString("https://miles.cvpartner.no/api/v1/users");
            var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesRaw);
            Log("Done - " + employees.Count + " users");
            
            foreach (var employee in employees)
            {
                var id = Domain.Services.IdService.CreateNewId();
                var url = "https://miles.cvpartner.no/api/v1/cvs/" + employee.UserId + "/" + employee.DefaultCvId;
                Log("Downloading CV for " + employee.Name + " on url " + url);

                var cv = JsonConvert.DeserializeObject<Cv>(client.DownloadString(url));

                //Picture employeePhoto = DownloadPhoto(cv.Image, cv.Name);
                Picture employeePhoto = null;

                var importEmployee = converter.ToImportFromCvPartner(id, cv, employee, employeePhoto, createdBy, correlationId);
                importData.Add(importEmployee);
            }
            return importData;
        }

        /// <summary>
        /// Import the entire miles cv-base to @miles. Should be run only once (no checks if employees are already added)
        /// </summary>
        /// <param name="accessToken">Super secret access token to the CVPartner api</param>
        /// <param name="createdBy">Employee with admin rights</param>
        /// <param name="openOfficeCreated">action when openoffice command is created</param>
        /// <param name="addEmployeeAction">action when addEmployee command is created </param>
        public bool ImportMilesComplete(string accessToken, Person createdBy, string correlationId, Action<OpenOffice> openOfficeCreated, Action<AddEmployee> addEmployeeAction, Action<AddCompanyAdmin> addCompanyAdmin, List<string> emailToAdminUsers)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token must be set!");
            }

            AddEmployeesCommands = new List<AddEmployee>();
            AddCompanyAdminsCommands = new List<AddCompanyAdmin>();

            var converter = new Converters.Convert("miles", createdBy, Log);
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"" + accessToken + "\"";

            Log("Download users from CvPartner...");
            var employees = JsonConvert.DeserializeObject<List<Employee>>(client.DownloadString("https://miles.cvpartner.no/api/v1/users"));
            Log("Done - " + employees.Count + " users");
            
            var offices = employees.Select(s => s.OfficeName).Distinct().ToList();

            OpenOfficeCommands = offices.Select(converter.ToOpenOffice).ToList();

            if (openOfficeCreated != null)
            {
                foreach (var openOfficeCommand in OpenOfficeCommands)
                {
                    try
                    {
                        openOfficeCreated(openOfficeCommand);
                    }
                    catch (Exception ex)
                    {
                        Log("OpenOffice handler failed:\n\n " + ex);
                    }
                }
            }
            
            Log(OpenOfficeCommands.Count() + " offices found");
            
            foreach (var employee in employees)
            {
                var id = Domain.Services.IdService.CreateNewId();
                var url = "https://miles.cvpartner.no/api/v1/cvs/" + employee.UserId + "/" + employee.DefaultCvId;
                Log("Downloading CV for " + employee.Name + " on url " + url);
                
                var cv = JsonConvert.DeserializeObject<Cv>(client.DownloadString(url));

                Picture emplyeePhoto = DownloadPhoto(cv.Image, cv.Name);

                var addEmployee = converter.ToAddEmployee(id, cv, employee, emplyeePhoto);

                Add(addEmployee, AddEmployeesCommands);
               
                if (addEmployeeAction != null)
                {
                    try
                    {
                        addEmployeeAction(addEmployee);
                    }
                    catch (Exception ex)
                    {
                        Log("AddEmployee handler failed:\n\n " + ex);
                    }
                }

                CheckIfShouldAddAsAdmin(addEmployee, addCompanyAdmin, emailToAdminUsers);
            }
            return true;
        }

        private Picture DownloadPhoto(Image image, string name)
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
                    picture = client.DownloadData(image.Url);
                    contentType = client.ResponseHeaders["Content-Type"];

                    var urlWithoutQueryParameters = image.Url.Substring(0, image.Url.IndexOf("?"));
                    extension = urlWithoutQueryParameters.Substring(image.Url.LastIndexOf("."))
                        .Replace(".", string.Empty);


                    Log("Found image of " + "." + extension + " format");
                }
                catch (Exception ex)
                {
                    Log("Error downloading image:\n\n " + ex);

                }
                if (picture != null)
                {
                    var hash = MD5.Create().ComputeHash(picture);
                    photo = new Picture(name, extension, picture, hash);
                }
            }
            return photo;
        }

        private void CheckIfShouldAddAsAdmin(AddEmployee addEmployee, Action<AddCompanyAdmin> addCompanyAdmin, List<string> emailToAdminUsers)
        {
            if (emailToAdminUsers != null)
            {
                if (emailToAdminUsers.Contains(addEmployee.Email))
                {
                    var cmd = new AddCompanyAdmin(addEmployee.CompanyId, addEmployee.GlobalId, DateTime.UtcNow,
                        addEmployee.CreatedBy, addEmployee.CorrelationId, Domain.Constants.IgnoreVersion);

                    Add(cmd, AddCompanyAdminsCommands);
                    if (addCompanyAdmin != null)
                    {
                        addCompanyAdmin(cmd);
                    }
                }
            }
        }

        public void Add<T>(T command, List<T> commands) where T:Command
        {
              commands.Add(command);
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
