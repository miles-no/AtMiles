namespace Contact.Infrastructure
{
    public interface IResolveUserIdentity
    {
        string ResolveUserIdentityByProviderId(string companyId, string provider, string providerId);

        string ResolveUserIdentityByEmail(string companyId, string provider, string email);
    }
}
