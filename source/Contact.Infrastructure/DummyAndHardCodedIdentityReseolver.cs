namespace Contact.Infrastructure
{
    public class DummyAndHardCodedIdentityReseolver : IResolveUserIdentity
    {
        public string ResolveUserIdentityByProviderId(string provider, string providerId)
        {
            if (provider == Domain.Constants.GoogleIdProvider)
            {
                //Roy
                if (providerId == "114551968215191716757")
                {
                    return "NjIsRmTxxkKbTGPDrs4kvQ";
                }

                //Stian
                if (providerId == "110095646841016563805")
                {
                    return "83J2cBApjkeMqgDt2mkKXQ";
                }
            }
            return string.Empty;
        }

        public string ResolveUserIdentityByEmail(string provider, string email)
        {
            if (provider == Domain.Constants.GoogleIdProvider)
            {
                //Roy
                if (email == "roy.veshovda@miles.no")
                {
                    return "m8IpkYKl0OgZ6MDGtXIfg";
                }

                //Stian
                if (email == "stian.edvardsen@miles.no")
                {
                    return "1auJVoQuq0uTASAL5BG4XQ";
                }
            }
            return string.Empty;
        }
    }
}
