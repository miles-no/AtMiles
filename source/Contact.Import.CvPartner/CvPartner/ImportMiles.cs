using System;
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
        private readonly ICommandSender commandSender;

        public ImportMiles(ICommandSender commandSender)
        {
            this.commandSender = commandSender;
        }

        /// <summary>
        /// Import the entire miles cv-base to @miles. Should be run only once (no checks if employees are already added)
        /// </summary>
        /// <param name="accessToken">Super secret access token to the CVPartner api</param>
        /// <param name="createdBy">Employee with admin rights</param>
        /// <param name="sendContinuously">send commands as soon as they are constructed (if set to false, all will be sent as the last thing)</param>
        public void ImportMilesComplete(string accessToken, Person createdBy, bool sendContinuously = true)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token must be set!");
            }

            var addEmployeesCommands = new List<AddEmployee>();

            var converter = new Converters.Convert("miles", createdBy, Log);
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"" + accessToken + "\"";

            Log("Download users from CvPartner...");
            var employees = JsonConvert.DeserializeObject<List<Employee>>(client.DownloadString("https://miles.cvpartner.no/api/v1/users"));
            Log("Done - " + employees.Count + " users");
            
            var offices = employees.Select(s => s.OfficeName).Distinct().ToList();

            var officeCommands = offices.Select(converter.ToOpenOffice).ToList();
            Log(officeCommands.Count() + " offices found");
            
            foreach (var office in officeCommands)
            {
                SendOrAdd(sendContinuously, office, officeCommands);
            }

            foreach (var employee in employees)
            {
                var url = "https://miles.cvpartner.no/api/v1/cvs/" + employee.UserId + "/" + employee.DefaultCvId;
                Log("Downloading CV for " + employee.Name);
                var cv = JsonConvert.DeserializeObject<Cv>(client.DownloadString(url));
                SendOrAdd(sendContinuously, converter.ToAddEmployee(cv, employee), addEmployeesCommands);
            }

            if (sendContinuously == false)
            {
                foreach (var officeCommand in officeCommands)
                {
                    Send(officeCommand);
                }
                
                foreach (var addEmployeesCommand in addEmployeesCommands)
                {
                    Send(addEmployeesCommand);
                }
            }
        }

        public void SendOrAdd<T>(bool sendNow, T command, List<T> commands) where T:Command
        {
            if (sendNow)
            {
                Send(command);   
            }
            else
            {
                commands.Add(command);
            }
        }

        public void Send<T>(T command) where T : Command
        {
            Log("Sending command " + command.GetType().Name);
            commandSender.Send(command);
        }
        
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
