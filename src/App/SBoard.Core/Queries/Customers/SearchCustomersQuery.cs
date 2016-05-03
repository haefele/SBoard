using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.Customers;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Queries.Customers
{
    public class SearchCustomersQuery : IQuery<IList<CustomerPreview>>
    {
        public string SearchText { get; }

        public SearchCustomersQuery(string searchText)
        {
            this.SearchText = searchText;
        }
    }

    public class SearchCustomersQueryHandler : IQueryHandler<SearchCustomersQuery, IList<CustomerPreview>>
    {
        private readonly ICentronService _centronService;

        public SearchCustomersQueryHandler(ICentronService centronService)
        {
            this._centronService = centronService;
        }

        public Task<IList<CustomerPreview>> ExecuteAsync(SearchCustomersQuery query)
        {
            return this._centronService.GetCustomersAsync(query.SearchText);
        }
    }
}