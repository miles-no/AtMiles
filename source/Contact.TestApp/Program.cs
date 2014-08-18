﻿using System;
using System.Collections.Generic;
using System.IO;
using Contact.Domain;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner;
using Contact.Infrastructure;
using Contact.TestApp.InMemoryReadModel;
using Company = Contact.Domain.Aggregates.Company;
using Employee = Contact.Domain.Aggregates.Employee;

namespace Contact.TestApp
{
    class Program
    {
        static void Main()
        {
            LongRunningProcess cmdWorker = null;
            LongRunningProcess readModelDemo = null;
            bool quit = false;

            while (!quit)
            {
                Console.WriteLine("");
                Console.WriteLine("=====================================================");
                Console.WriteLine("Miles Contact TestApp");
                Console.WriteLine("Select action:");
                Console.WriteLine("Quit: <q>");
                Console.WriteLine("Prepare EventStore with initial data: <1>");
                Console.WriteLine("Start command-handler: <2>");
                Console.WriteLine("Stop command-handler: <3>");
                Console.WriteLine("ReadModel super-simple demo: <A>");
                Console.WriteLine("ReadModel demo: <R>");

                var key = Console.ReadKey(true);

                Console.Clear();
                Console.WriteLine("Result:");
                Console.WriteLine("");
                switch (key.Key)
                {

                    case ConsoleKey.Q:
                        Console.WriteLine("Quiting...");
                        StopReadModel(readModelDemo);
                        quit = true;
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Test1();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        cmdWorker = Test2();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Test3(cmdWorker);
                        break;
                    case ConsoleKey.R:
                        Console.WriteLine("Starting Readmodel processing");
                        readModelDemo = ReadModelDemo();
                        break;
                    case ConsoleKey.A:
                        new ReadModelSearchDemo().TestSubscription();
                        break;
                    case ConsoleKey.S:
                        StopReadModel(readModelDemo);
                        break;

                    //Add more functions here

                    default:
                        Console.WriteLine("Unknown key: " + key.Key);
                        break;
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine("Exited");
        }

        private static void StopReadModel(LongRunningProcess readModelDemo)
        {
            if (readModelDemo != null)
            {
                Console.WriteLine("Stopping readmodel...");
                readModelDemo.Stop();
                Console.WriteLine("ReadModel Stopped");
            }
        }


        private static void Test3(LongRunningProcess worker)
        {
            if (worker != null)
            {
                worker.Stop();
                Console.WriteLine("Command-worker Stopped");
            }
        }

        private static LongRunningProcess Test2()
        {
            const string host = "milescontact.cloudapp.net";
            const string eSUsername = "admin";
            const string eSPassword = "changeit";

            const string rMqUsername = "miles";
            const string rMqPassword = "GoGoMilesContact";

            var companyRepository = new EventStoreRepository<Company>(host, null, eSUsername, eSPassword);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, eSUsername, eSPassword);

            var cmdHandler = MainCommandHandlerFactory.Initialize(companyRepository, employeeRepository);

            var cmdReceiver = new RabbitMqCommandHandler(cmdHandler);

            var worker = new QueueWorker(host, rMqUsername, rMqPassword, "Commands", null, cmdReceiver.MessageHandler);

            worker.Start();

            Console.WriteLine("Command-worker Started");
            return worker;
        }

        private static void Test1()
        {
            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            //const string password = "GoGoMilesContact";
            const string password = "changeit";

            const string companyId = "miles";
            const string companyName = "Miles";
            const string officeId = "SVG";
            const string officeName = "Stavanger";
            var officeAddress = new Address("Øvre Holmegate 1, 3. etasje", "4006", "Stavanger");

            const string systemId = "SYSTEM";
            const string systemLastName = "SYSTEM";
            var systemDateOfBirth = new DateTime(2014, 9, 1);
            const string systemJobTitle = "System";

            var companyRepository = new EventStoreRepository<Company>(host, null, username, password);
            var globalRepository = new EventStoreRepository<Global>(host, null, username, password);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, username, password);
            var global = new Global();

            const string initCorrelationId = "SYSTEM INIT";

            var system = new Employee();
            system.CreateNew(companyId, companyName, officeId, officeName, systemId, string.Empty, string.Empty, systemLastName, systemDateOfBirth, systemJobTitle, string.Empty, String.Empty, null, null, new Person("SYSTEM", "SYSTEM"), initCorrelationId);

            var systemAsPerson = new Person(system.Id, system.Name);

            var company = new Company();
            company.CreateNewCompany(companyId, companyName, officeId, officeName, officeAddress, system.Id, system.Name, DateTime.UtcNow, systemAsPerson, initCorrelationId);

            global.AddCompany(company, systemAsPerson, initCorrelationId);

            try
            {
                globalRepository.Save(global, Constants.NewVersion);
                employeeRepository.Save(system, Constants.NewVersion);
                companyRepository.Save(company, Constants.NewVersion);
                Console.WriteLine("Successfully created seed info");
            }
            catch (Exception error)
            {
                Console.WriteLine("Exception: " + error);
                return;
            }

            Console.WriteLine("Starting to import data from CVpartner");

            string cvPartnerToken;
#if testing
            cvPartnerToken = File.ReadAllText("D:\\miles\\key.txt");
#endif
            var import = new ImportMiles();
            var companyCommandHandler = new CompanyCommandHandler(companyRepository, employeeRepository);

            
            var userEmailsToPromotoToCompanyAdmin = new List<string> {"roy.veshovda@miles.no", "stian.edvardsen@miles.no"};

            import.ImportMilesComplete(cvPartnerToken, systemAsPerson, companyCommandHandler.Handle, companyCommandHandler.Handle, companyCommandHandler.Handle, userEmailsToPromotoToCompanyAdmin);
        }

        private static LongRunningProcess ReadModelDemo()
        {
            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            //const string password = "GoGoMilesContact";
            const string password = "changeit";

            var handler = new ReadModelHandler();

            var repository = new InMemoryRepository();

            var employeeHandler = new EmployeeHandler(repository);
            handler.RegisterHandler<EmployeeCreated>(employeeHandler.Handle);
            handler.RegisterHandler<EmployeeTerminated>(employeeHandler.Handle);

            var companyHandler = new CompanyHandler(repository);
            handler.RegisterHandler<CompanyCreated>(companyHandler.Handle);
            handler.RegisterHandler<CompanyAdminAdded>(companyHandler.Handle);
            handler.RegisterHandler<CompanyAdminRemoved>(companyHandler.Handle);
            handler.RegisterHandler<EmployeeAdded>(companyHandler.Handle);
            handler.RegisterHandler<EmployeeRemoved>(companyHandler.Handle);
            handler.RegisterHandler<OfficeAdminAdded>(companyHandler.Handle);
            
            handler.RegisterHandler<OfficeAdminRemoved>(companyHandler.Handle);
            handler.RegisterHandler<OfficeClosed>(companyHandler.Handle);
            handler.RegisterHandler<OfficeOpened>(companyHandler.Handle);

            var demo = new EventStoreDispatcher(host, username, password, handler, new ConsoleLogger(), () => PrintInMemoRyRepository(repository));
            demo.Start();

            return demo;
        }

        private static void PrintInMemoRyRepository(InMemoryRepository repository)
        {
            Console.WriteLine("ReadModel contains:");

            Console.WriteLine("Employees: {0}", repository.Employees.Count);

            Console.WriteLine("Companies: {0}",repository.Companies.Count);
            Console.WriteLine();
            foreach (var company in repository.Companies)
            {
                Console.WriteLine("Company: {0}:",company.Name);
                Console.WriteLine("\t{0} Admins", company.Admins.Count);
                Console.WriteLine("\t{0} Offices", company.Offices.Count);
                Console.WriteLine();
                foreach (var office in company.Offices)
                {
                    Console.WriteLine("\tOffice: {0}:", office.Name);
                    Console.WriteLine("\t\tAdmins: {0}:", office.Admins.Count);
                    Console.WriteLine("\t\tEmployees: {0}:", office.Employees.Count);
                }
            }
            Console.WriteLine("***********************************************************");
        }
    }
}