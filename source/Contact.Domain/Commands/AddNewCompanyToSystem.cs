using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Commands
{
    public class AddNewCompanyToSystem : Command
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string FirstOfficeId;
        public readonly string FirstOfficeName;
        public readonly Address FirstOfficeAddress;
        public readonly SimpleUserInfo[] InitialAdmins;


        public AddNewCompanyToSystem(string companyId, string companyName, string firstOfficeId, string firstOfficeName, Address firstOfficeAddress, SimpleUserInfo[] initialAdmins, DateTime created, Person createdBy, string correlationId, int basedOnVersion)
            : base(created, createdBy, correlationId, basedOnVersion)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            FirstOfficeId = firstOfficeId;
            FirstOfficeName = firstOfficeName;
            FirstOfficeAddress = firstOfficeAddress;
            InitialAdmins = initialAdmins;
        }
    }
}
