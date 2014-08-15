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
            string userInfo = message.GlobalId + ": " +
                              Domain.Services.NameService.GetName(message.FirstName, message.MiddleName,
                                  message.LastName);
            _userList.Add(userInfo);
            Console.WriteLine("User {0} : {1}", _userList.Count, userInfo);
        }
    }
}
