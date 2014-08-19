namespace Contact.Infrastructure
{
    public class DummyAndHardCodedIdentityReseolver : IResolveUserIdentity
    {
        private const string IdRoy = "cuEhnrDMrU+Oi3RApT70JA";
        private const string IdStian = "RVgYYRLtSE6+u+JleUoFQg";
        public string ResolveUserIdentityByProviderId(string provider, string providerId)
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

        public string ResolveUserIdentityByEmail(string provider, string email)
        {
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
