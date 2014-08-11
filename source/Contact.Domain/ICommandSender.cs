namespace Contact.Domain
{
    public interface ICommandSender
    {
        bool Send<T>(T command) where T : Command;
    }
}
