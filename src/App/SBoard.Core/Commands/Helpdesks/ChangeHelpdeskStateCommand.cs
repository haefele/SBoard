using System;
using System.Reactive;
using System.Threading.Tasks;

namespace SBoard.Core.Commands.Helpdesks
{
    public class ChangeHelpdeskStateCommand : ICommand<Unit>
    {
        public int HelpdeskI3D { get; }
        public int HelpdeskStateI3D { get; }
        
        public ChangeHelpdeskStateCommand(int helpdeskI3D, int helpdeskStateI3D)
        {
            this.HelpdeskI3D = helpdeskI3D;
            this.HelpdeskStateI3D = helpdeskStateI3D;
        }
    }

    public class ChangeHelpdeskStateCommandHandler : ICommandHandler<ChangeHelpdeskStateCommand, Unit>
    {
        public ChangeHelpdeskStateCommandHandler()
        {
            
        }

        public async Task<Unit> ExecuteAsync(ChangeHelpdeskStateCommand command)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            return Unit.Default;
        }
    }
}