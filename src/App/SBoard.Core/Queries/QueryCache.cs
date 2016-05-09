using System.Threading.Tasks;
using UwCore.Services.ApplicationState;

namespace SBoard.Core.Queries
{
    public class QueryCache : IQueryCache
    {
        private readonly IApplicationStateService _applicationStateService;

        public QueryCache(IApplicationStateService applicationStateService)
        {
            this._applicationStateService = applicationStateService;
        }

        public Task<bool> TryGetCachedQueryAsync<TResult>(IQuery<TResult> query, out TResult result)
        {
            string key = this.GetKey(query);

            result = this._applicationStateService.Get<TResult>(key, ApplicationState.Local);
            var isDefault = object.Equals(result, default(TResult));

            return Task.FromResult(isDefault == false);
        }

        public Task CacheQueryAsync<TResult>(IQuery<TResult> query, TResult data)
        {
            var key = this.GetKey(query);

            this._applicationStateService.Set(key, data, ApplicationState.Local);
            return Task.CompletedTask;
        }

        private string GetKey<TResult>(IQuery<TResult> query)
        {
            return string.Format("QueryCache:{0}:{1}", query.GetType().Name, query.GetHashCode());
        }
    }
}