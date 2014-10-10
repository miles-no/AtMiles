namespace Contact.Infrastructure
{
    public interface IResolveNameOfUser
    {
        string ResolveUserNameById(string companyId, string userId);
    }
}
