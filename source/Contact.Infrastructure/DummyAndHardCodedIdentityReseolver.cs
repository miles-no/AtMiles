namespace Contact.Infrastructure
{
    public class DummyAndHardCodedIdentityReseolver : IResolveUserIdentity
    {
        private const string IdRoy = "iaeRaej2UKNfXaGw7XwUA";
        private const string IdStian = "kHBA9PUhREqPImf6iSCG7A";
        public string ResolveUserIdentityByProviderId(string companyId, string provider, string providerId)
        {
            if (provider == Domain.Constants.GoogleIdProvider)
            {
                //Roy
                if (providerId == "114551968215191716757")
                {
                    return IdRoy;
                }

                //Stian
                if (providerId == "110095646841016563805")
                {
                    return IdStian;
                }
            }
            return string.Empty;
        }

        
        public string AttachLoginToUser(string companyid, string provider, string providerId, string email, out string message)
        {
            
            message = "Everything is fine, roger roger";

            if (provider == Domain.Constants.GoogleIdProvider)
            {
                //Roy
                if (email == "roy.veshovda@miles.no")
                {
                    return IdRoy;
                }

                //Stian
                if (email == "stian.edvardsen@miles.no")
                {
                    return IdStian;
                }
            }
            return string.Empty;
            
        }
    }
}
