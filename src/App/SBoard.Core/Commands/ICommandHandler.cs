using System.Threading.Tasks;

namespace SBoard.Core.Commands
{
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(TCommand command);
    }
}