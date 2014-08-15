using System;
using System.Collections.Generic;
using Contact.Domain;
using Contact.Domain.Events.Employee;

namespace Contact.TestApp
{
    public class AllUsersReadModelHandler : Handles<Domain.Events.Employee.EmployeeCreated>
    {
        private readonly List<string> _userList;

        public AllUsersReadModelHandler()
        {
            _userList = new List<string>();
        }
        public void Handle(EmployeeCreated message)
        {
            _userList.Add(message.GlobalId + ": " +Domain.Services.NameService.GetName(message.FirstName, message.MiddleName, message.LastName));
        }

        public void PrintAllUsersToConsole()
        {
            Console.WriteLine("Users ({0})", _userList.Count);
            foreach (var user in _userList)
            {
                Console.WriteLine(user);
            }
        }
    }
}
