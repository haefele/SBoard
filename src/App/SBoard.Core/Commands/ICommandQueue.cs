using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using SBoard.Core.Commands.Events;

namespace SBoard.Core.Commands
{
    public interface ICommandQueue
    {
        Task EnqueueAsync<TResult>(ICommand<TResult> command);
    }

    public class CommandQueue : ICommandQueue
    {
        private readonly WinRTContainer _container;
        private readonly IEventAggregator _eventAggregator;

        public CommandQueue(WinRTContainer container, IEventAggregator eventAggregator)
        {
            this._container = container;
            this._eventAggregator = eventAggregator;
        }

        public async Task EnqueueAsync<TResult>(ICommand<TResult> command)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
            var handler = this._container.GetInstance(handlerType, null);
            var method = handler.GetType().GetMethod(nameof(ICommandHandler<ICommand<TResult>,TResult>.ExecuteAsync));

            TResult result = await (Task<TResult>)method.Invoke(handler, new object[] { command });

            this._eventAggregator.PublishOnCurrentThread(new CommandExecuted<TResult>(command, result));
        }
    }
}