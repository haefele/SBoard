namespace SBoard.Core.Commands.Events
{
    public class CommandExecuted<TCommandResult>
    {
        public ICommand<TCommandResult> Command { get; }
        public TCommandResult Result { get; }

        public CommandExecuted(ICommand<TCommandResult> command, TCommandResult result)
        {
            this.Command = command;
            this.Result = result;
        }
    }
}