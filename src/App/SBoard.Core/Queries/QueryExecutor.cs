using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SBoard.Core.Queries
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly WinRTContainer _container;

        public QueryExecutor(WinRTContainer container)
        {
            this._container = container;
        }

        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = this._container.GetInstance(handlerType, null);

            var method = handler.GetType().GetMethod("ExecuteAsync");
            //TODO: Add caching if an exception occurs
            return await (Task<TResult>)method.Invoke(handler, new object[] {query});
        }
    }
}