using System;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SBoard.Core.Queries
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly WinRTContainer _container;
        private readonly IQueryCache _queryCache;

        public QueryExecutor(WinRTContainer container, IQueryCache queryCache)
        {
            this._container = container;
            this._queryCache = queryCache;
        }

        public async Task<QueryResult<TResult>> ExecuteAsync<TResult>(IQuery<TResult> query)
        {
            try
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
                var handler = this._container.GetInstance(handlerType, null);
                var method = handler.GetType().GetMethod("ExecuteAsync");

                TResult result = await (Task<TResult>)method.Invoke(handler, new object[] {query});

                await this._queryCache.CacheQueryAsync(query, result);

                return new QueryResult<TResult>(false, result);
            }
            catch
            {
                TResult result;
                if (await this._queryCache.TryGetCachedQueryAsync(query, out result))
                {
                    return new QueryResult<TResult>(true, result);
                }

                throw;
            }
        }
    }
}