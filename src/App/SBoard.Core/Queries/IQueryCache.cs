using System.Threading.Tasks;

namespace SBoard.Core.Queries
{
    public interface IQueryCache
    {
        Task<bool> TryGetCachedQueryAsync<TResult>(IQuery<TResult> query, out TResult result);
        Task CacheQueryAsync<TResult>(IQuery<TResult> query, TResult data);
    }
}