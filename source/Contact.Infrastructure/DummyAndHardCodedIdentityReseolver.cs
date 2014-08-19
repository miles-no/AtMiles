namespace Contact.Infrastructure
{
    public class DummyAndHardCodedIdentityReseolver : IResolveUserIdentity
    {
        public string ResolveUserIdentity(string provider, string providerId)
        {
            if (provider == Domain.Constants.GoogleIdProvider)
            {
                //Roy
                if (providerId == "114551968215191716757")
                {
                    return "m8IpkYKl0OgZ6MDGtXIfg";
                }

                //Stian
                if (providerId == "110095646841016563805")
                {
                    return "1auJVoQuq0uTASAL5BG4XQ";
                }
            }
            return string.Empty;
        }
    }
}
