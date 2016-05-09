using System.Threading.Tasks;

namespace SBoard.Core.Queries
{
    public interface IQueryExecutor
    {
        Task<QueryResult<TResult>> ExecuteAsync<TResult>(IQuery<TResult> query);
    }

    public class QueryResult<TResult>
    {
        public QueryResult(bool isStale, TResult result)
        {
            this.IsStale = isStale;
            this.Result = result;
        }

        public bool IsStale { get; }
        public TResult Result { get; }
    }
}