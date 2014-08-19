namespace Contact.Infrastructure
{
    public interface IResolveUserIdentity
    {
        string ResolveUserIdentity(string provider, string providerId);
    }
}
