using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddNewCompanyToSystem : Command
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string FirstOfficeName;
        public readonly SimpleUserInfo[] InitialAdmins;


        public AddNewCompanyToSystem(string companyId, string companyName, string firstOfficeName, SimpleUserInfo[] initialAdmins, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            FirstOfficeName = firstOfficeName;
            InitialAdmins = initialAdmins;
        }
    }
}
