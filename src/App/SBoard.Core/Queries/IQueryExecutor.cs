using System.Threading.Tasks;

namespace SBoard.Core.Queries
{
    public interface IQueryExecutor
    {
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query);
    }
}