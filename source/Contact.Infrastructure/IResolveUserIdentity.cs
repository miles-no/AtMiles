namespace Contact.Infrastructure
{
    public interface IResolveUserIdentity
    {
        string ResolveUserIdentityByProviderId(string provider, string providerId);

        string ResolveUserIdentityByEmail(string provider, string email);
    }
}
