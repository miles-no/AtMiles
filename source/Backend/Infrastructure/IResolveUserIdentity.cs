namespace no.miles.at.Backend.Infrastructure
{
    public interface IResolveUserIdentity
    {
        string ResolveUserIdentitySubject(string companyId, string subject);
    }
}
