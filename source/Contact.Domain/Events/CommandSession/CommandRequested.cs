namespace Contact.Domain.Events.CommandSession
{
    public class CommandRequested : Event
    {
        public readonly Command Command;

        public CommandRequested(Command command)
            : base(command.Created, command.CreatedBy, command.CorrelationId)
        {
            Command = command;
        }
    }
}
