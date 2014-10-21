namespace no.miles.at.Backend.Domain
{
    public interface ICommandSender
    {
        void Send<T>(T command) where T : Command;
    }
}
