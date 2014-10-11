namespace no.miles.at.Backend.Infrastructure
{
    public interface IResolveNameOfUser
    {
        string ResolveUserNameById(string companyId, string userId);
    }
}
