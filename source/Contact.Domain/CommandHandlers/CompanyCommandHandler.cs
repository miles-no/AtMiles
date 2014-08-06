using Contact.Domain.Commands;

namespace Contact.Domain.CommandHandlers
{
    public class CompanyCommandHandler :
        Handles<AddCompanyAdmin>,
        Handles<RemoveCompanyAdmin>,
        Handles<RemoveOfficeAdmin>,
        Handles<AddOfficeAdmin>,
        Handles<OpenOffice>,
        Handles<CloseOffice>
    {
        public void Handle(AddCompanyAdmin message)
        {
            //TODO: Implement
        }

        public void Handle(RemoveCompanyAdmin message)
        {
            //TODO: Implement
        }

        public void Handle(RemoveOfficeAdmin message)
        {
            //TODO: Implement
        }

        public void Handle(AddOfficeAdmin message)
        {
            //TODO: Implement
        }

        public void Handle(OpenOffice message)
        {
            //TODO: Implement
        }

        public void Handle(CloseOffice message)
        {
            //TODO: Implement
        }
    }
}
