namespace Contact.Infrastructure
{
    public interface IResolveUserIdentity
    {
        string ResolveUserIdentitySubject(string companyId, string subject);
    }
}
