﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner.Models.Cv;
using Contact.Import.CvPartner.CvPartner.Models.Employee;
using Newtonsoft.Json;

namespace Contact.Import.CvPartner.CvPartner
{
    public class ImportMiles
    {

        public List<AddEmployee> AddEmployeesCommands { get; set; }
        public List<OpenOffice> OpenOfficeCommands { get; set; }
        public List<AddCompanyAdmin> AddCompanyAdminsCommands { get; set; }

        


        /// <summary>
        /// Import the entire miles cv-base to @miles. Should be run only once (no checks if employees are already added)
        /// </summary>
        /// <param name="accessToken">Super secret access token to the CVPartner api</param>
        /// <param name="createdBy">Employee with admin rights</param>
        /// <param name="sendContinuously">send commands as soon as they are constructed (if set to false, all will be sent as the last thing)</param>
        /// <param name="openOfficeCreated">action when openoffice command is created</param>
        /// <param name="addEmployeeAction">action when addEmployee command is created </param>
        public bool ImportMilesComplete(string accessToken, Person createdBy, Action<OpenOffice> openOfficeCreated, Action<AddEmployee> addEmployeeAction, Action<AddCompanyAdmin> addCompanyAdmin, List<string> emailToAdminUsers)
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
                var addEmployee = converter.ToAddEmployee(id, cv, employee);
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