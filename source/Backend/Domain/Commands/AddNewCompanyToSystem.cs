using System;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Commands
{
    public class AddNewCompanyToSystem : Command
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly SimpleUserInfo[] InitialAdmins;


        public AddNewCompanyToSystem(string companyId, string companyName, SimpleUserInfo[] initialAdmins, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            InitialAdmins = initialAdmins;
        }
    }
}
